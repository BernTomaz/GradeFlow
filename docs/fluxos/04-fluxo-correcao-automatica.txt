GradeFlow - Fluxo 04 - Correcao Automatica

Objetivo
Corrigir automaticamente as respostas de uma submissao com base no tipo de questao e no gabarito estruturado.

Usuario principal
Professor.

Fluxo funcional
1. Professor abre uma submissao.
2. Clica em corrigir.
3. Backend busca a submissao.
4. Backend carrega respostas do aluno.
5. Backend carrega questoes e gabaritos.
6. CorrectionService percorre cada resposta.
7. CorrectionService escolhe a estrategia correta.
8. Strategy calcula acerto, pontuacao e feedback.
9. Sistema salva CorrectionResult.
10. Sistema soma a nota final.
11. Sistema atualiza status da submissao.

Motor de correcao
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

Numerica:
- Converter resposta do aluno para numero.
- Comparar com resposta correta.
- Usar tolerancia.
- Formula: abs(respostaAluno - respostaCorreta) <= tolerancia.

Texto curto:
- Converter para minusculo.
- Remover acentos.
- Remover pontuacao.
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
- Exibicao de nota e feedback

Testes recomendados
- xUnit
- FluentAssertions
- Testes unitarios para cada strategy

Regra importante
Controller nao deve corrigir resposta.
Controller deve chamar o service.
O service deve chamar o motor de correcao.

