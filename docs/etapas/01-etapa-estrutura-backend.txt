GradeFlow - Etapa 01 - Estrutura do Backend

Objetivo
Criar a base da API em .NET com separacao minima de responsabilidades.

Tecnologias
- .NET 10
- ASP.NET Core Web API
- C#
- Swagger

Estrutura recomendada
src/
  GradeFlow.Api/
  GradeFlow.Application/
  GradeFlow.Domain/
  GradeFlow.Infrastructure/

Responsabilidades
GradeFlow.Api:
- Controllers
- Swagger
- Middlewares
- Configuracoes da aplicacao

GradeFlow.Application:
- Services
- DTOs
- Casos de uso
- Validacoes

GradeFlow.Domain:
- Entidades
- Enums
- Regras de negocio
- Interfaces do motor de correcao

GradeFlow.Infrastructure:
- Entity Framework Core
- DbContext
- Migrations
- Repositorios
- Configuracao de banco

Tarefas
- Criar solution GradeFlow
- Criar os projetos da solution
- Adicionar referencias entre os projetos
- Configurar Swagger
- Configurar injeção de dependência
- Criar estrutura inicial de pastas

Comandos base
dotnet new sln -n GradeFlow
dotnet new webapi -n GradeFlow.Api
dotnet new classlib -n GradeFlow.Application
dotnet new classlib -n GradeFlow.Domain
dotnet new classlib -n GradeFlow.Infrastructure

Ponto de atencao
Nao complique demais a arquitetura no inicio.
A separacao deve ajudar, nao travar o desenvolvimento.

