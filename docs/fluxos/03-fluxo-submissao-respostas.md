GradeFlow - Fluxo 03 - Submissao de Respostas

Objetivo
Permitir inserir respostas de um aluno para uma avaliacao.

Usuario principal
No MVP, o professor pode inserir as respostas manualmente.
Depois, o aluno pode ter login proprio para enviar respostas.

Fluxo funcional
1. Professor abre uma avaliacao.
2. Clica em nova submissao.
3. Informa nome e email do aluno.
4. Sistema lista as questoes da avaliacao.
5. Professor insere a resposta de cada questao.
6. Sistema salva a submissao com status pendente.
7. Sistema permite executar a correcao.

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
- Validacao de questoes pertencentes a avaliacao

Banco:
- EF Core
- Relacionamento Assignment 1:N Submission
- Relacionamento Submission 1:N StudentAnswer

Frontend:
- Angular Reactive Forms
- FormArray para respostas
- HttpClient
- Component de submissao manual

Validacoes
- StudentName obrigatorio
- Cada resposta deve estar vinculada a uma Question existente
- A Question precisa pertencer a Assignment informada
- Resposta numerica deve poder ser convertida para numero quando a questao for Numeric

Regra importante
No MVP, submissao manual e suficiente.
Importacao por Excel ou CSV deve ficar para depois.

