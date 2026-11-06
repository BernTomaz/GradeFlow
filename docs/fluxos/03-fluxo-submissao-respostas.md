GradeFlow - Fluxo 03 - Submissão de Respostas

Objetivo
Permitir inserir respostas de um aluno para uma avaliação.

usuário principal
No MVP, o professor pode inserir as respostas manualmente.
O Aluno já possui login e pode consultar avaliações e questões sem gabarito, enviar respostas e alterar respostas individuais de suas próprias submissões.
O catálogo atual não possui distribuição por turma; o acesso às submissões é restrito pelo StudentUserId, não pelo nome ou email informado.

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

Validações
- StudentName obrigatório
- Cada resposta deve estar vinculada a uma Question existente
- A Question precisa pertencer a Assignment informada
- Resposta numérica deve poder ser convertida para número quando a questão for Numeric

Regra importante
O fluxo manual continua sendo a base do sistema.
Importação em lote já existe via CSV no fluxo de importação.
Importação por Excel ainda não foi implementada.

Atualização de respostas
- PUT /api/submissions/{submissionId}/questions/{questionId}/answer altera uma resposta própria ou gerenciada.
- PUT /api/submissions/{id}/student altera nome e email informados, sem transferir a propriedade.
- PUT /api/submissions/{id} substitui todas as respostas e exige Admin ou Professor responsável.
- Uma resposta alterada perde a pontuação anterior e deixa a submissão pendente de nova correção.
