# Testes

## Backend

```powershell
dotnet clean GradeFlow.slnx -m:1
dotnet restore GradeFlow.slnx
dotnet build GradeFlow.slnx --no-restore -m:1
dotnet test GradeFlow.slnx --no-build -m:1
```

## Frontend

```powershell
cd src\GradeFlow.Web
npm ci
npm run build
npm test -- --watch=false --browsers=ChromeHeadless
```

Nesta validação, `npm ci` é intencional para reproduzir o comportamento esperado no pipeline.

No ambiente local em Windows/OneDrive, `dotnet clean GradeFlow.slnx` pode falhar ao remover arquivos temporários quando o MSBuild executa projetos em paralelo. Use `-m:1` localmente; no CI, prefira workspace limpo e não dependa de `clean` sem necessidade.

Os testes cobrem:

- Serviços de aplicação
- Regras de negócio
- Estratégias de correção
- Fluxos de autenticação
