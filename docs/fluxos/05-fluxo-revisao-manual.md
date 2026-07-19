GradeFlow - Fluxo 05 - Revisão Manual

Objetivo
Permitir que o professor revise e ajuste a correção automática quando necessário.

usuário principal
Professor.

Fluxo funcional
1. Professor abre o resultado da submissão.
2. Sistema mostra questão, resposta do aluno, gabarito e nota sugerida.
3. Professor altera a pontuação se necessário.
4. Professor adiciona feedback manual.
5. Professor marca resposta como revisada.
6. Sistema atualiza a nota da resposta.
7. Sistema recalcula a nota final da submissão.
8. Sistema registra log da revisão.

Entidades envolvidas
StudentAnswer:
- ScoreAwarded
- IsCorrect
- Feedback
- NeedsReview

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

Endpoint recomendado
PUT /api/student-answers/{answerId}/review

DTO sugerido
ReviewStudentAnswerRequest:
- ScoreAwarded
- Feedback
- IsCorrect

ReviewStudentAnswerResponse:
- AnswerId
- SubmissionId
- ScoreAwarded
- Feedback
- IsCorrect
- NeedsReview
- FinalScore

Tecnologias envolvidas
Backend:
- ASP.NET Core Controller
- Application Service
- EF Core transaction
- Auditoria em CorrectionLog

Banco:
- Atualizacao de StudentAnswer
- Insercao em CorrectionLog
- Recalculo de FinalScore em Submission

Frontend:
- Angular Component para revisão
- Textarea para feedback
- Input numérico para pontuação
- Visualização lado a lado de resposta e gabarito

Cuidados
Sem log, o sistema não consegue explicar por que uma nota foi alterada.
Em sistema de correção, auditabilidade é parte central do produto.

Regra importante
A IA, se entrar no futuro, deve sugerir.
A decisão final em questões abertas deve continuar revisavel.

