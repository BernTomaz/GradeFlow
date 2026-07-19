GradeFlow - Fluxo 04 - Correção Automática

Objetivo
Corrigir automaticamente as respostas de uma submissão com base no tipo de questão e no gabarito estruturado.

usuário principal
Professor.

Fluxo funcional
1. Professor abre uma submissão.
2. Clica em corrigir.
3. Backend busca a submissão.
4. Backend carrega respostas do aluno.
5. Backend carrega questões e gabaritos.
6. CorrectionService percorre cada resposta.
7. CorrectionService escolhe a estratégia correta.
8. Strategy calcula acerto, pontuação e feedback.
9. Sistema salva CorrectionResult.
10. Sistema soma a nota final.
11. Sistema atualiza status da submissão.

Motor de correção
Interface:
ICorrectionStrategy

Strategies do MVP:
- MultipleChoiceCorrectionStrategy
- TrueFalseCorrectionStrategy
- NumericCorrectionStrategy
- ShortTextCorrectionStrategy

Regras do MVP
Multipla escolha:
- Comparar alternativa do aluno com alternativa correta.

Verdadeiro ou falso:
- Comparar true/false.

Numérica:
- Converter resposta do aluno para número.
- Comparar com resposta correta.
- Usar tolerância.
- Fórmula: abs(respostaAluno - respostaCorreta) <= tolerância.

Texto curto:
- Converter para minusculo.
- Remover acentos.
- Remover pontuação.
- Remover espacos extras.
- Comparar com resposta correta e respostas aceitas.

Entidade principal
CorrectionResult:
- Id
- StudentAnswerId
- IsCorrect
- ScoreAwarded
- Feedback
- CorrectionType
- CreatedAt

Endpoint recomendado
POST /api/submissions/{submissionId}/correct

DTOs sugeridos
CorrectionResponse:
- SubmissionId
- FinalScore
- MaxScore
- Results

StudentAnswerCorrectionResponse:
- QuestionId
- Answer
- IsCorrect
- ScoreAwarded
- Feedback
- NeedsReview

Tecnologias envolvidas
Backend:
- C#
- Strategy Pattern
- Dependency Injection
- Application Service
- Domain Service

Banco:
- EF Core
- SaveChanges transacional
- Busca otimizada com Include ou projection

Frontend:
- Angular Service
- Tela de resultado
- Botao de corrigir
- Exibição de nota e feedback

Testes recomendados
- xUnit
- FluentAssertions
- Testes unitarios para cada strategy

Regra importante
Controller não deve corrigir resposta.
Controller deve chamar o service.
O service deve chamar o motor de correção.

