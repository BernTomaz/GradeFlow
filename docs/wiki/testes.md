# Testes

## Estrategia

Os testes automatizados cobrem regras de negocio, servicos de aplicacao, estrategias de correcao e fluxos de autenticacao.

## Backend

Ferramentas:

- xUnit
- FluentAssertions

Comando:

```powershell
dotnet test GradeFlow.slnx
```

## Frontend

Comando:

```powershell
cd src/GradeFlow.Web
npm test -- --watch=false
```
