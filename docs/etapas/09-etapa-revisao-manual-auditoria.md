GradeFlow - Etapa 09 - Revisão Manual e Auditoria

Objetivo
Permitir que o professor revise respostas e ajuste notas quando necessário.

Tecnologias
- ASP.NET Core
- Entity Framework Core
- Angular
- Reactive Forms

Fluxo
1. Professor abre resultado da submissão.
2. Sistema mostra questão, resposta do aluno, gabarito e nota sugerida.
3. Professor altera nota, se necessário.
4. Professor adiciona feedback.
5. Sistema salva revisão.
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
- Criar endpoint de revisão
- Atualizar StudentAnswer
- Recalcular Submission.FinalScore
- Criar tela de revisão no Angular

Ponto de atenção
Auditoria é essencial.
O sistema precisa explicar por que uma nota foi atribuída ou alterada.

## Ajustes de auditoria e acesso (06/11/2026)

Correções automáticas e revisões manuais registram o usuário executor em `ReviewedByUserId`; recorreções acrescentam histórico em `CorrectionLog`. Não há reconstrução do histórico antigo nem migration. Aluno lê questões sem gabarito e opera somente suas submissões. O catálogo ainda não possui distribuição por turma. A validação completa pela interface do Aluno e o deploy público permanecem pendentes.
