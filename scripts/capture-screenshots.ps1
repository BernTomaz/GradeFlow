param(
    [string]$BaseUrl = "http://localhost:4200",
    [string]$ApiUrl = "http://localhost:8080",
    [string]$Email = "teste@gradeflow.local",
    [string]$Password = "Teste@123",
    [string]$OutputDir = "docs/screenshots"
)

$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$login = @{ email = $Email; password = $Password } | ConvertTo-Json
$auth = Invoke-RestMethod -Method Post -Uri "$ApiUrl/api/auth/login" -ContentType "application/json" -Body $login
$authJson = $auth | ConvertTo-Json -Depth 8 -Compress

$chrome = @(
    "$env:ProgramFiles\Google\Chrome\Application\chrome.exe",
    "$env:ProgramFiles(x86)\Google\Chrome\Application\chrome.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles(x86)\Microsoft\Edge\Application\msedge.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $chrome) {
    throw "Chrome ou Edge nao encontrado."
}

$profile = Join-Path ([IO.Path]::GetTempPath()) ("gradeflow-cdp-" + [Guid]::NewGuid())
$process = Start-Process -FilePath $chrome -PassThru -WindowStyle Hidden -ArgumentList @(
    "--headless=new",
    "--disable-gpu",
    "--hide-scrollbars",
    "--remote-debugging-port=9222",
    "--user-data-dir=$profile",
    "about:blank"
)

try {
    $ready = $false
    for ($attempt = 0; $attempt -lt 20; $attempt++) {
        Start-Sleep -Milliseconds 500
        try {
            Invoke-RestMethod -Uri "http://127.0.0.1:9222/json/version" | Out-Null
            $ready = $true
            break
        }
        catch {
            if ($process.HasExited) {
                throw "Chrome encerrou antes de abrir a porta de automacao."
            }
        }
    }

    if (-not $ready) {
        throw "Chrome nao abriu a porta de automacao."
    }

    $tab = Invoke-RestMethod -Method Put -Uri "http://127.0.0.1:9222/json/new?about:blank"
    $socket = [Net.WebSockets.ClientWebSocket]::new()
    $socket.ConnectAsync([Uri]$tab.webSocketDebuggerUrl, [Threading.CancellationToken]::None).GetAwaiter().GetResult()
    $nextId = 0

    function Send-Cdp {
        param([string]$Method, [object]$Params = @{})

        $script:nextId++
        $message = @{ id = $script:nextId; method = $Method; params = $Params } | ConvertTo-Json -Depth 10 -Compress
        $bytes = [Text.Encoding]::UTF8.GetBytes($message)
        $socket.SendAsync([ArraySegment[byte]]::new($bytes), [Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None).GetAwaiter().GetResult()

        do {
            $buffer = [byte[]]::new(1048576)
            $result = $socket.ReceiveAsync([ArraySegment[byte]]::new($buffer), [Threading.CancellationToken]::None).GetAwaiter().GetResult()
            $response = [Text.Encoding]::UTF8.GetString($buffer, 0, $result.Count) | ConvertFrom-Json
        } until ($response.id -eq $script:nextId)

        if ($response.error) {
            throw "$Method failed: $($response.error.message)"
        }

        return $response.result
    }

    Send-Cdp "Page.enable" | Out-Null
    Send-Cdp "Runtime.enable" | Out-Null
    Send-Cdp "Emulation.setDeviceMetricsOverride" @{
        width = 1440
        height = 1000
        deviceScaleFactor = 1
        mobile = $false
    } | Out-Null

    function Save-Screenshot {
        param([string]$Name, [string]$Path, [bool]$Authenticated = $true)

        Send-Cdp "Page.navigate" @{ url = "$BaseUrl/$Path" } | Out-Null
        Start-Sleep -Seconds 2

        if ($Authenticated) {
            $escaped = ($authJson -replace "\\", "\\") -replace "'", "\'"
            Send-Cdp "Runtime.evaluate" @{
                expression = "sessionStorage.setItem('gradeflow.auth', '$escaped'); location.href = '$BaseUrl/$Path';"
                awaitPromise = $false
            } | Out-Null
            Start-Sleep -Seconds 3
        }

        $capture = Send-Cdp "Page.captureScreenshot" @{ format = "png"; captureBeyondViewport = $true }
        [IO.File]::WriteAllBytes((Join-Path $OutputDir $Name), [Convert]::FromBase64String($capture.data))
    }

    Save-Screenshot "login.png" "login" $false
    Save-Screenshot "dashboard.png" "dashboard"
    Save-Screenshot "avaliacoes.png" "assignments"
    Save-Screenshot "nova-avaliacao.png" "assignments/new"
    Save-Screenshot "novo-usuario.png" "users/new"
    Save-Screenshot "sobre.png" "about"

    $repoRoot = Split-Path -Parent $PSScriptRoot
    $python = @(
        "$env:USERPROFILE\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe",
        "python"
    ) | Where-Object {
        if ($_ -eq "python") {
            (Get-Command python -ErrorAction SilentlyContinue) -ne $null
        }
        else {
            Test-Path $_
        }
    } | Select-Object -First 1

    if ($python) {
        & $python (Join-Path $repoRoot "scripts/create-demo-gif.py")
    }
}
finally {
    if ($socket) { $socket.Dispose() }
    if ($process -and -not $process.HasExited) {
        $process.Kill()
        $process.WaitForExit(5000) | Out-Null
    }
    if (Test-Path $profile) {
        Remove-Item -LiteralPath $profile -Recurse -Force -ErrorAction SilentlyContinue
    }
}
