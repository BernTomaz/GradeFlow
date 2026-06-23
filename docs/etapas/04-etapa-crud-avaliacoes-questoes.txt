GradeFlow - Etapa 04 - CRUD de Avaliacoes, Questoes e Gabaritos

Objetivo
Criar os primeiros endpoints para cadastrar avaliacoes, questoes e gabaritos estruturados.

Tecnologias
- ASP.NET Core Controllers
- DTOs
- Application Services
- Entity Framework Core
- Swagger

Endpoints de avaliacoes
POST /api/assignments
GET  /api/assignments
GET  /api/assignments/{id}
PUT  /api/assignments/{id}
DELETE /api/assignments/{id}

Endpoints de questoes
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
- Validar dados obrigatorios
- Salvar gabarito junto com questao
- Testar endpoints no Swagger

Ponto de atencao
Gabarito precisa ser estruturado por tipo de questao.
Nao trate tudo como uma string solta.

