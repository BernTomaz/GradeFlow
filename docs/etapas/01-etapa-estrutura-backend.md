GradeFlow - Etapa 01 - Estrutura do Backend

Objetivo
Criar a base da API em .NET com separação mínima de responsabilidades.

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
- Configurações da aplicação

GradeFlow.Application:
- Services
- DTOs
- Casos de uso
- Validações

GradeFlow.Domain:
- Entidades
- Enums
- Regras de negócio
- Interfaces do motor de correção

GradeFlow.Infrastructure:
- Entity Framework Core
- DbContext
- Migrations
- Repositorios
- Configuração de banco

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

Ponto de atenção
Não complique demais a arquitetura no início.
A separação deve ajudar, não travar o desenvolvimento.

