GradeFlow - Fluxo 02 - Criar questões e Gabarito

Objetivo
Permitir que o professor cadastre questões dentro de uma avaliação e configure o gabarito de cada uma.

usuário principal
Professor.

Fluxo funcional
1. Professor abre uma avaliação.
2. Clica em adicionar questão.
3. Informa o enunciado.
4. Escolhe o tipo da questão.
5. Define a pontuação.
6. Cadastra o gabarito.
7. Configura regras específicas, se necessário.
8. Sistema salva questão e gabarito.

Tipos de questão do MVP
- MultipleChoice
- TrueFalse
- Numeric
- ShortText

Entidades principais
Question:
- Id
- AssignmentId
- Text
- Type
- Points
- Order
- CorrectionConfigJson

AnswerKey:
- Id
- QuestionId
- CorrectAnswer
- AcceptedAnswersJson
- KeywordsJson
- Tolerance
- FeedbackCorrect
- FeedbackIncorrect

Endpoints recomendados
GET  /api/assignments/{assignmentId}/questions
POST /api/assignments/{assignmentId}/questions
GET  /api/questions/{id}
PUT  /api/questions/{id}
DELETE /api/questions/{id}

DTOs sugeridos
CreateQuestionRequest:
- Text
- Type
- Points
- Order
- AnswerKey

CreateAnswerKeyRequest:
- CorrectAnswer
- AcceptedAnswers
- Keywords
- Tolerance
- FeedbackCorrect
- FeedbackIncorrect

QuestionResponse:
- Id
- AssignmentId
- Text
- Type
- Points
- Order
- AnswerKey

Tecnologias envolvidas
Backend:
- ASP.NET Core Controller
- Application Service
- Enum para QuestionType
- Serialização JSON para configurações específicas

Banco:
- EF Core
- Relacionamento Assignment 1:N Question
- Relacionamento Question 1:1 AnswerKey

Frontend:
- Angular Reactive Forms
- Select para tipo de questão
- Campos dinamicos conforme o tipo
- Angular Service com HttpClient

Cuidados
Não trate todo gabarito como texto simples.
questão numérica precisa de tolerância.
questão de texto curto precisa de normalizacao.
questão de multipla escolha precisa comparar alternativas.

Regra importante
O gabarito estruturado e o que diferencia esse projeto de um sistema fraco.

