# Correcao Automatica

## Visao Geral

O motor de correcao automatica seleciona uma estrategia conforme o tipo da questao.

## Fluxo

1. Receber submissao.
2. Identificar o tipo da questao.
3. Selecionar a estrategia apropriada.
4. Executar a correcao.
5. Gerar resultado.
6. Registrar log.

## Strategy Pattern

Cada tipo de questao possui uma estrategia propria de correcao. Isso evita colocar regras de correcao nos controllers e facilita evoluir cada tipo separadamente.

## Estrategias

### Multiple Choice Strategy

Corrige questoes de multipla escolha comparando a alternativa enviada com o gabarito.

### True/False Strategy

Corrige questoes de verdadeiro ou falso.

### Numeric Strategy

Corrige respostas numericas usando tolerancia configuravel.

### Short Text Strategy

Corrige texto curto usando normalizacao de acentos, pontuacao e espacos.
