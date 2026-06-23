GradeFlow - Etapa 09 - Testes

Objetivo
Garantir que o motor de correcao funciona corretamente.

Tecnologias
- xUnit
- FluentAssertions
- Moq, se necessario

Prioridade dos testes
Teste primeiro as strategies de correcao.
Depois teste o CorrectionService.
Controllers podem ficar para uma etapa posterior.

Testes obrigatorios
MultipleChoiceCorrectionStrategy:
- Deve acertar quando alternativa for igual ao gabarito.
- Deve errar quando alternativa for diferente do gabarito.

TrueFalseCorrectionStrategy:
- Deve comparar true/false corretamente.

NumericCorrectionStrategy:
- Deve acertar quando resposta estiver dentro da tolerancia.
- Deve errar quando resposta estiver fora da tolerancia.
- Deve marcar erro quando resposta nao for numerica.

ShortTextCorrectionStrategy:
- Deve ignorar maiusculas e minusculas.
- Deve ignorar acentos.
- Deve ignorar pontuacao.
- Deve aceitar respostas alternativas.

CorrectionService:
- Deve corrigir todas as respostas de uma submissao.
- Deve somar a nota final.
- Deve salvar CorrectionResult.
- Deve atualizar status da submissao.

Ponto de atencao
Se o motor de correcao nao tem teste, o projeto perde confianca.
Essa e a parte mais importante para provar maturidade tecnica.

