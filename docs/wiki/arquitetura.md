# Arquitetura

## Visao Geral

O GradeFlow usa arquitetura em camadas para separar regras de negocio, casos de uso, persistencia e exposicao HTTP.

## Camadas

### Domain

Responsavel pelas regras de negocio e entidades principais:

- Assignment
- Question
- AnswerKey
- Submission
- StudentAnswer
- CorrectionResult
- CorrectionLog
- User

### Application

Responsavel pelos casos de uso da aplicacao:

- Services
- DTOs
- Interfaces
- Strategies de correcao
- Regras de aplicacao

### Infrastructure

Responsavel pelo acesso a dados:

- Entity Framework Core
- Repositories
- Migrations
- Configuracoes do banco

### API

Responsavel pela exposicao dos endpoints REST:

- Controllers
- Middlewares
- Autenticacao
- Autorizacao
- Swagger/OpenAPI

### Web

Frontend Angular responsavel pela interface do usuario.
