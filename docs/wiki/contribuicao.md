# Contribuicao

## Como Executar Localmente

### Backend

```powershell
dotnet restore
dotnet run --project src/GradeFlow.Api --launch-profile https
```

API:

```txt
https://localhost:7013
```

Swagger:

```txt
https://localhost:7013/swagger
```

### Frontend

```powershell
cd src/GradeFlow.Web
npm install
npm start
```

Aplicacao:

```txt
http://localhost:4200
```

### Banco de Dados

Configure a connection string em:

```txt
src/GradeFlow.Api/appsettings.json
```

Aplique as migrations:

```powershell
dotnet ef database update --project src\GradeFlow.Infrastructure --startup-project src\GradeFlow.Api
```
