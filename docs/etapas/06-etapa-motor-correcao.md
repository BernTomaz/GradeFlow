GradeFlow - Etapa 06 - Motor de correção

Objetivo
Criar o núcleo do sistema: a correção automática por tipo de questão.

Tecnologias
- C#
- Strategy Pattern
- Dependency Injection
- Application Service
- Entity Framework Core

Interface principal
ICorrectionStrategy

Responsabilidade da interface
Receber uma questão, seu gabarito e a resposta do aluno.
Retornar se está correto, pontuação, feedback e necessidade de revisão.

Strategies do MVP
- MultipleChoiceCorrectionStrategy
- TrueFalseCorrectionStrategy
- NumericCorrectionStrategy
- ShortTextCorrectionStrategy

CorrectionService deve
- Buscar submissão
- Buscar respostas do aluno
- Buscar questões
- Buscar gabaritos
- Identificar tipo da questão
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
- Converter resposta para número.
- Aplicar tolerância.
- Fórmula: abs(respostaAluno - respostaCorreta) <= tolerância.

ShortText:
- Normalizar texto.
- Remover acentos.
- Ignorar maiúsculas/minúsculas.
- Remover pontuação.
- Comparar com resposta correta e respostas aceitas.

Ponto de atenção
Controller não deve corrigir resposta.
Controller chama service.
Service chama motor de correção.

