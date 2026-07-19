GradeFlow - Fluxo 03 - submissão de Respostas

Objetivo
Permitir inserir respostas de um aluno para uma avaliação.

usuário principal
No MVP, o professor pode inserir as respostas manualmente.
Depois, o aluno pode ter login proprio para enviar respostas.

Fluxo funcional
1. Professor abre uma avaliação.
2. Clica em nova submissão.
3. Informa nome e email do aluno.
4. Sistema lista as questões da avaliação.
5. Professor insere a resposta de cada questão.
6. Sistema salva a submissão com status pendente.
7. Sistema permite executar a correção.

Entidades principais
Submission:
- Id
- AssignmentId
- StudentName
- StudentEmail
- Status
- FinalScore
- SubmittedAt
- CorrectedAt
- ReviewedAt

StudentAnswer:
- Id
- SubmissionId
- QuestionId
- Answer
- ScoreAwarded
- IsCorrect
- Feedback
- NeedsReview

Endpoints recomendados
POST /api/assignments/{assignmentId}/submissions
GET  /api/assignments/{assignmentId}/submissions
GET  /api/submissions/{id}

DTOs sugeridos
CreateSubmissionRequest:
- StudentName
- StudentEmail
- Answers

CreateStudentAnswerRequest:
- QuestionId
- Answer

SubmissionResponse:
- Id
- AssignmentId
- StudentName
- StudentEmail
- Status
- FinalScore
- Answers

Tecnologias envolvidas
Backend:
- ASP.NET Core Controller
- Application Service
- DTOs compostos
- validação de questões pertencentes a avaliação

Banco:
- EF Core
- Relacionamento Assignment 1:N Submission
- Relacionamento Submission 1:N StudentAnswer

Frontend:
- Angular Reactive Forms
- FormArray para respostas
- HttpClient
- Component de submissão manual

Validacoes
- StudentName obrigatorio
- Cada resposta deve estar vinculada a uma Question existente
- A Question precisa pertencer a Assignment informada
- Resposta numérica deve poder ser convertida para número quando a questão for Numeric

Regra importante
O fluxo manual continua sendo a base do sistema.
importação em lote já existe via CSV no fluxo de importação.
importação por Excel ainda Não foi implementada.
