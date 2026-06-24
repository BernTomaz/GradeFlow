GradeFlow - Fluxo 01 - Criar Avaliacao

Objetivo
Permitir que o professor crie uma avaliacao, atividade, prova ou trabalho.

Usuario principal
Professor.

Fluxo funcional
1. Professor acessa a tela de avaliacoes.
2. Clica em criar nova avaliacao.
3. Informa titulo, descricao, disciplina e status.
4. Sistema salva a avaliacao.
5. Sistema exibe a avaliacao criada na lista.

Campos principais
Assignment:
- Id
- Title
- Description
- Subject
- TotalPoints
- Status
- CreatedAt
- UpdatedAt

Campos que podem ficar para depois
- TeacherId
- CourseId
- ClassroomId

Endpoints recomendados
POST /api/assignments
GET  /api/assignments
GET  /api/assignments/{id}
PUT  /api/assignments/{id}
DELETE /api/assignments/{id}

DTOs sugeridos
CreateAssignmentRequest:
- Title
- Description
- Subject

UpdateAssignmentRequest:
- Title
- Description
- Subject
- Status

AssignmentResponse:
- Id
- Title
- Description
- Subject
- TotalPoints
- Status
- CreatedAt

Tecnologias envolvidas
Backend:
- ASP.NET Core Controller
- C#
- DTOs
- Application Service

Banco:
- Entity Framework Core
- SQL Server ou PostgreSQL
- Migration para tabela Assignments

Frontend:
- Angular Component
- Angular Service
- Reactive Forms
- HttpClient

Validacoes
- Title obrigatorio
- Title com tamanho maximo
- Description opcional
- Subject opcional no MVP

Regra importante
A avaliacao ainda nao precisa ter login no MVP.
Primeiro valide se o fluxo de criacao funciona.

