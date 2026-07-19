GradeFlow - Etapa 08 - Testes

Objetivo
Garantir que o motor de correção funciona corretamente.

Tecnologias
- xUnit
- FluentAssertions
- Moq, se necessário

Prioridade dos testes
Teste primeiro as strategies de correção.
Depois teste o CorrectionService.
Controllers podem ficar para uma etapa posterior.

Testes obrigatórios
MultipleChoiceCorrectionStrategy:
- Deve acertar quando alternativa for igual ao gabarito.
- Deve errar quando alternativa for diferente do gabarito.

TrueFalseCorrectionStrategy:
- Deve comparar true/false corretamente.

NumericCorrectionStrategy:
- Deve acertar quando resposta estiver dentro da tolerância.
- Deve errar quando resposta estiver fora da tolerância.
- Deve marcar erro quando resposta não for numérica.

ShortTextCorrectionStrategy:
- Deve ignorar maiúsculas e minúsculas.
- Deve ignorar acentos.
- Deve ignorar pontuação.
- Deve aceitar respostas alternativas.

CorrectionService:
- Deve corrigir todas as respostas de uma submissão.
- Deve somar a nota final.
- Deve salvar CorrectionResult.
- Deve atualizar status da submissão.

Ponto de atenção
Se o motor de correção não tem teste, o projeto perde confiança.
Essa é a parte mais importante para provar maturidade técnica.
