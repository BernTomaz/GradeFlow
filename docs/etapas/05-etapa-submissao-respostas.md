GradeFlow - Etapa 05 - Submissao de Respostas

Objetivo
Permitir inserir respostas de um aluno para uma avaliacao.

Tecnologias
- ASP.NET Core Controllers
- DTOs compostos
- Application Services
- Entity Framework Core

Fluxo
1. Informar avaliacao.
2. Informar nome e email do aluno.
3. Enviar lista de respostas.
4. Salvar Submission.
5. Salvar StudentAnswers.
6. Deixar submissao com status Pending.

Endpoint principal
POST /api/assignments/{assignmentId}/submissions

Endpoints auxiliares
GET /api/assignments/{assignmentId}/submissions
GET /api/submissions/{id}

DTOs recomendados
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

Validacoes
- StudentName obrigatorio
- Assignment precisa existir
- QuestionId precisa existir
- Question precisa pertencer a Assignment
- Resposta nao deve ser nula

Ponto de atencao
No MVP, a submissao pode ser manual.
Submissao manual continua sendo o fluxo base.
Importacao em lote ja existe por CSV.
Importacao por Excel continua como recurso futuro, se houver necessidade.
