GradeFlow - Etapa 08 - Revisao Manual e Auditoria

Objetivo
Permitir que o professor revise respostas e ajuste notas quando necessario.

Tecnologias
- ASP.NET Core
- Entity Framework Core
- Angular
- Reactive Forms

Fluxo
1. Professor abre resultado da submissao.
2. Sistema mostra questao, resposta do aluno, gabarito e nota sugerida.
3. Professor altera nota, se necessario.
4. Professor adiciona feedback.
5. Sistema salva revisao.
6. Sistema recalcula nota final.
7. Sistema registra log.

Entidade adicional
CorrectionLog:
- Id
- SubmissionId
- QuestionId
- CorrectionType
- OriginalAnswer
- NormalizedAnswer
- ExpectedAnswer
- Score
- Message
- CreatedAt
- ReviewedByUserId

Endpoint principal
PUT /api/student-answers/{answerId}/review

DTO recomendado
ReviewStudentAnswerRequest:
- ScoreAwarded
- Feedback
- IsCorrect

Tarefas
- Criar CorrectionLog
- Criar migration
- Criar endpoint de revisao
- Atualizar StudentAnswer
- Recalcular Submission.FinalScore
- Criar tela de revisao no Angular

Ponto de atencao
Auditoria e essencial.
O sistema precisa explicar por que uma nota foi atribuida ou alterada.

