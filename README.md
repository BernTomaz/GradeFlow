# GradeFlow

Sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

## Objetivo

O GradeFlow tem como objetivo apoiar professores, avaliadores e instituições no processo de correção de atividades, provas e trabalhos.

A proposta do projeto é permitir o cadastro de avaliações com questões estruturadas, configuração de gabaritos por tipo de questão e uso de um motor de correção automática para calcular notas de forma mais consistente, rastreável e auditável.

## Status do Projeto

> Projeto em desenvolvimento ativo. O backend já possui o motor de correção, camadas de aplicação/domínio e testes automatizados implementados. O frontend está em construção.

### ✅ Implementado

**Domínio e correção**
- Modelagem de domínio (`Assignment`, `Question`, `AnswerKey`, `StudentAnswer`, `Submission`, `CorrectionResult`, `CorrectionLog`)
- Motor de correção automática via *Strategy Pattern*, com uma estratégia por tipo de questão
- Correção de múltipla escolha, verdadeiro/falso, numérica (com tolerância) e texto curto (com normalização de acentos, pontuação e espaços)
- Correção da prova inteira ou de uma questão isolada (recorreção pontual)
- Log de correção por submissão (`correction-logs`), permitindo auditoria de como cada nota foi calculada
- Revisão manual de resposta específica, com endpoint dedicado (`review`)

**Backend**
- Camada de aplicação com serviços (`AssignmentService`, `CorrectionService`, `QuestionService`, `SubmissionService`) e interfaces de repositório
- API REST completa (CRUD) com controllers para `Assignments`, `Questions` e `Submissions`
- Persistência com Entity Framework Core + SQL Server, com 3 migrations já aplicadas (`InitialCreate`, `RequireStudentAnswerAnswer`, `AddCorrectionLogs`)
- Swagger/OpenAPI habilitado em ambiente de desenvolvimento
- CORS configurado para o frontend Angular
- Suite de testes automatizados (xUnit + FluentAssertions) cobrindo serviços de aplicação e estratégias de correção

**Frontend**
- Aplicação Angular 20 (standalone components) com camada `core` (serviços de API e models tipados) e `features` por domínio
- Telas de listagem, criação e detalhe de avaliações (`assignments`)
- Tela de criação de questões (`questions`)
- Telas de criação e detalhe de submissões (`submissions`)
- Tela de resultado de correção (`correction`)

### 🔜 Planejado

- Fluxo de revisão manual de respostas integrado na UI (hoje o endpoint existe, a tela ainda não)
- Autenticação e perfis de usuário (professor/avaliador)
- Pipeline de CI (build + testes automatizados a cada push)
- Exportação/relatório de notas por turma

### Tipos de questão suportados

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização

## Tecnologias

### Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core

### Frontend

- Angular
- TypeScript
- Reactive Forms
- HttpClient

### Banco de Dados

- SQL Server

### Testes

- xUnit
- FluentAssertions

## Arquitetura

O projeto segue uma separação em camadas inspirada em Clean Architecture.
A ordem de implementação de cada etapa está documentada em `docs/etapas/`.

```txt
src/
  GradeFlow.Api/             # Controllers, middlewares e ponto de entrada da API
  GradeFlow.Application/     # Serviços, casos de uso, DTOs e estratégias de correção
  GradeFlow.Domain/          # Entidades, enums e contratos de domínio
  GradeFlow.Infrastructure/  # Persistência (EF Core) e implementações de repositório
  GradeFlow.Web/             # Frontend Angular
tests/
  GradeFlow.Tests/           # Testes de unidade (xUnit + FluentAssertions)
```

## Como Rodar Localmente

### Backend

No Visual Studio, selecione o perfil `https` do projeto `GradeFlow.Api`.

Ou rode pelo terminal:

```powershell
dotnet run --project src\GradeFlow.Api --launch-profile https
```

A API deve ficar disponível em:

```txt
https://localhost:7013
```

A documentação interativa (Swagger) fica em `https://localhost:7013/swagger`.

#### Endpoints principais

| Recurso | Endpoints |
|---|---|
| Assignments | `GET/POST /api/assignments`, `GET/PUT/DELETE /api/assignments/{id}` |
| Questions | `GET/POST /api/assignments/{assignmentId}/questions`, `GET/PUT/DELETE /api/questions/{id}` |
| Submissions | `GET/POST /api/assignments/{assignmentId}/submissions`, `GET/PUT/DELETE /api/submissions/{id}` |
| Correção | `POST /api/submissions/{id}/correct`, `POST /api/submissions/{id}/questions/{questionId}/correct` |
| Revisão manual | `PUT /api/student-answers/{answerId}/review` |
| Auditoria | `GET /api/submissions/{id}/correction-logs` |

### Frontend

Em outro terminal:

```powershell
cd src\GradeFlow.Web
npm start
```

Abra:

```txt
http://localhost:4200
```

O frontend usa `proxy.conf.json` para encaminhar chamadas `/api` para `https://localhost:7013`.

### Testes

```powershell
dotnet test
```
