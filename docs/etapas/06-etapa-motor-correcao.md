GradeFlow - Etapa 06 - Motor de Correcao

Objetivo
Criar o nucleo do sistema: a correcao automatica por tipo de questao.

Tecnologias
- C#
- Strategy Pattern
- Dependency Injection
- Application Service
- Entity Framework Core

Interface principal
ICorrectionStrategy

Responsabilidade da interface
Receber uma questao, seu gabarito e a resposta do aluno.
Retornar se esta correto, pontuacao, feedback e necessidade de revisao.

Strategies do MVP
- MultipleChoiceCorrectionStrategy
- TrueFalseCorrectionStrategy
- NumericCorrectionStrategy
- ShortTextCorrectionStrategy

CorrectionService deve
- Buscar submissao
- Buscar respostas do aluno
- Buscar questoes
- Buscar gabaritos
- Identificar tipo da questao
- Escolher a strategy correta
- Corrigir cada resposta
- Salvar CorrectionResult
- Atualizar StudentAnswer
- Calcular FinalScore
- Atualizar status da Submission

Endpoint principal
POST /api/submissions/{submissionId}/correct

Regras por tipo
MultipleChoice:
- Comparar alternativa do aluno com alternativa correta.

TrueFalse:
- Comparar valor verdadeiro/falso.

Numeric:
- Converter resposta para numero.
- Aplicar tolerancia.
- Formula: abs(respostaAluno - respostaCorreta) <= tolerancia.

ShortText:
- Normalizar texto.
- Remover acentos.
- Ignorar maiusculas/minusculas.
- Remover pontuacao.
- Comparar com resposta correta e respostas aceitas.

Ponto de atencao
Controller nao deve corrigir resposta.
Controller chama service.
Service chama motor de correcao.

