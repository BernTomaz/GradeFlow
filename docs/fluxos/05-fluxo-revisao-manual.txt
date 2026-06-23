GradeFlow - Fluxo 05 - Revisao Manual

Objetivo
Permitir que o professor revise e ajuste a correcao automatica quando necessario.

Usuario principal
Professor.

Fluxo funcional
1. Professor abre o resultado da submissao.
2. Sistema mostra questao, resposta do aluno, gabarito e nota sugerida.
3. Professor altera a pontuacao se necessario.
4. Professor adiciona feedback manual.
5. Professor marca resposta como revisada.
6. Sistema atualiza a nota da resposta.
7. Sistema recalcula a nota final da submissao.
8. Sistema registra log da revisao.

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
- Angular Component para revisao
- Textarea para feedback
- Input numerico para pontuacao
- Visualizacao lado a lado de resposta e gabarito

Cuidados
Sem log, o sistema nao consegue explicar por que uma nota foi alterada.
Em sistema de correcao, auditabilidade e parte central do produto.

Regra importante
A IA, se entrar no futuro, deve sugerir.
A decisao final em questoes abertas deve continuar revisavel.

