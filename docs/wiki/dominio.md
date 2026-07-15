# Dominio

## Assignment

Representa uma avaliacao.

Responsabilidades:

- Agrupar questoes.
- Permitir submissoes.
- Definir propriedade da avaliacao.

Relacionamentos:

- Questions
- Submissions
- User

## Question

Representa uma questao dentro de uma avaliacao.

Tipos suportados:

- Multipla escolha
- Verdadeiro ou falso
- Numerica
- Texto curto

## AnswerKey

Representa o gabarito usado pelo motor de correcao.

## Submission

Representa o envio de respostas de um aluno.

Fluxo:

1. Aluno envia respostas.
2. Sistema executa correcao automatica.
3. Resultado e armazenado.
4. Respostas podem passar por revisao manual.

## StudentAnswer

Representa uma resposta individual dentro de uma submissao.

## CorrectionResult

Armazena a nota e o resultado consolidado da correcao.

## CorrectionLog

Registra eventos relevantes da correcao para auditoria.

## User

Representa um usuario autenticado.

Perfis:

- Admin
- Teacher
- Student
