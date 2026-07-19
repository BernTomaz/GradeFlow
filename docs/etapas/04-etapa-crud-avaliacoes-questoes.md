GradeFlow - Etapa 04 - CRUD de avaliações, questões e Gabaritos

Objetivo
Criar os primeiros endpoints para cadastrar avaliações, questões e gabaritos estruturados.

Tecnologias
- ASP.NET Core Controllers
- DTOs
- Application Services
- Entity Framework Core
- Swagger

Endpoints de avaliações
POST /api/assignments
GET  /api/assignments
GET  /api/assignments/{id}
PUT  /api/assignments/{id}
DELETE /api/assignments/{id}

Endpoints de questões
GET  /api/assignments/{assignmentId}/questions
POST /api/assignments/{assignmentId}/questions
GET  /api/questions/{id}
PUT  /api/questions/{id}
DELETE /api/questions/{id}

DTOs recomendados
CreateAssignmentRequest
UpdateAssignmentRequest
AssignmentResponse

CreateQuestionRequest
UpdateQuestionRequest
QuestionResponse
CreateAnswerKeyRequest

Tarefas
- Criar AssignmentController
- Criar QuestionController
- Criar DTOs
- Criar AssignmentService
- Criar QuestionService
- Validar dados obrigatórios
- Salvar gabarito junto com questão
- Testar endpoints no Swagger

Ponto de atenção
Gabarito precisa ser estruturado por tipo de questão.
Não trate tudo como uma string solta.

