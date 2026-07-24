# Configuração e Execução Local

## Pré-requisitos

Antes de executar o projeto, instale:

- .NET 10 SDK
- Node.js 20.19 ou superior
- Angular CLI
- SQL Server LocalDB, Express ou Developer
- Git
- EF Core Tools

Instalação da ferramenta do Entity Framework Core:

```powershell
dotnet tool install --global dotnet-ef
```

Verificar versões instaladas:

```powershell
dotnet --version
node --version
npm --version
dotnet ef --version
```

## Instalação

Clone o repositório:

```bash
git clone https://github.com/BernTomaz/GradeFlow.git
cd GradeFlow
```

Restaurar dependências do backend:

```powershell
dotnet restore
```

Instalar dependências do frontend:

```powershell
cd src/GradeFlow.Web
npm install
```

Use `npm install` no desenvolvimento local. Para validação limpa ou CI, use `npm ci`, que instala exatamente as versões do `package-lock.json`.

## Banco de Dados

O projeto utiliza SQL Server com Entity Framework Core Migrations.

Edite o arquivo:

```txt
src/GradeFlow.Api/appsettings.json
```

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GradeFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Este `appsettings.json` é destinado ao ambiente de desenvolvimento local. Em ambientes produtivos, utilize variáveis de ambiente, User Secrets ou serviços de gerenciamento de segredos.

Aplicar migrations em desenvolvimento local:

```powershell
dotnet ef database update --project src\GradeFlow.Infrastructure --startup-project src\GradeFlow.Api
```

Para Docker, homologação e produção, gere um script idempotente e aplique de forma controlada antes de publicar a aplicação:

```powershell
.\scripts\database\generate-migration-script.ps1
```

Detalhes da estratégia: [operacao/migrations.md](operacao/migrations.md).

## Migrations Existentes

- InitialCreate
- RequireStudentAnswerAnswer
- AddCorrectionLogs
- AddUsers
- AddUserOwnership

## Backend

No Visual Studio, selecione o perfil:

```txt
https
```

Antes de iniciar a API, configure a chave JWT localmente com user-secrets:

```powershell
dotnet user-secrets set "Jwt:Key" "troque-por-uma-chave-local-com-32-ou-mais-caracteres" --project src\GradeFlow.Api
```

As configurações obrigatórias de JWT são `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` e `Jwt:ExpirationMinutes`.

`Issuer`, `Audience` e tempo de expiração podem ficar em `appsettings.json`; a chave (`Jwt:Key`) deve vir de user-secrets em desenvolvimento ou variável de ambiente em produção:

```txt
Jwt__Key=troque-por-uma-chave-segura-do-ambiente
```

Em produção, configure também a connection string e as origens permitidas do frontend por variáveis de ambiente:

```txt
ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...
Cors__AllowedOrigins__0=https://seu-frontend.example.com
```

Use `.env.example` apenas como referência. Não versione `.env` real nem credenciais.

Regras atuais de senha para cadastro e alteração de senha:

- mínimo de 8 caracteres;
- pelo menos uma letra maiúscula;
- pelo menos um número;
- pelo menos um caractere especial.

O login limita tentativas inválidas por email e IP. Após 5 falhas dentro de 1 minuto, a API bloqueia novas tentativas temporariamente.

Executar a API:

```powershell
dotnet run --project src\GradeFlow.Api --launch-profile https
```

A API estará disponível em:

```txt
https://localhost:7013
```

Documentação Swagger:

```txt
https://localhost:7013/swagger
```

Health check:

```txt
https://localhost:7013/health
```

Para testar rotas protegidas pelo Swagger, faça login em `/api/auth/login`, copie o campo `token`, clique em `Authorize` e informe apenas o token. A interface adiciona o prefixo `Bearer` automaticamente.

## Frontend

Abra um novo terminal:

```powershell
cd src\GradeFlow.Web
npm start
```

Aplicação disponível em:

```txt
http://localhost:4200
```

O frontend utiliza o arquivo `proxy.conf.json` para encaminhar automaticamente chamadas `/api` para a API local.

## Docker Compose

Crie um `.env` local a partir do `.env.example` e ajuste `MSSQL_SA_PASSWORD`, `APP_DB_PASSWORD` e `JWT_KEY`.

O arquivo `.env` real é local e não deve ser versionado.

```powershell
docker compose build
docker compose up
```

Serviços locais:

- Frontend: `http://localhost:4200`
- API: `http://localhost:8080`
- Health check: `http://localhost:8080/health`
- SQL Server: acessível apenas pela rede interna do Docker

As migrations de banco não são aplicadas automaticamente pelo Compose.
Gere e aplique o script idempotente conforme [operacao/migrations.md](operacao/migrations.md).

Para gerar backup do banco Docker:

```powershell
.\scripts\database\backup-docker.ps1
```
