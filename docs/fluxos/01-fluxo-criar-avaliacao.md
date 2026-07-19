GradeFlow - Fluxo 01 - Criar avaliação

Objetivo
Permitir que o professor crie uma avaliação, atividade, prova ou trabalho.

usuário principal
Professor.

Fluxo funcional
1. Professor acessa a tela de avaliações.
2. Clica em criar nova avaliação.
3. Informa título, descrição, disciplina e status.
4. Sistema salva a avaliação.
5. Sistema exibe a avaliação criada na lista.

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
- CourseId
- ClassroomId

Campo adicionado depois da autenticação
- TeacherUserId

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
- SQL Server
- Migration para tabela Assignments

Frontend:
- Angular Component
- Angular Service
- Reactive Forms
- HttpClient

Validações
- Title obrigatório
- Title com tamanho máximo
- Description opcional
- Subject opcional no MVP

Regra importante
A avaliação ainda não precisa ter login no MVP.
Primeiro valide se o fluxo de criação funciona.
Depois da etapa de login, o backend passa a validar ownership da avaliação.
