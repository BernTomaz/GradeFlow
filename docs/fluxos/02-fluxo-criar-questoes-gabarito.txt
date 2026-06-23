GradeFlow - Fluxo 02 - Criar Questoes e Gabarito

Objetivo
Permitir que o professor cadastre questoes dentro de uma avaliacao e configure o gabarito de cada uma.

Usuario principal
Professor.

Fluxo funcional
1. Professor abre uma avaliacao.
2. Clica em adicionar questao.
3. Informa o enunciado.
4. Escolhe o tipo da questao.
5. Define a pontuacao.
6. Cadastra o gabarito.
7. Configura regras especificas, se necessario.
8. Sistema salva questao e gabarito.

Tipos de questao do MVP
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
- Serializacao JSON para configuracoes especificas

Banco:
- EF Core
- Relacionamento Assignment 1:N Question
- Relacionamento Question 1:1 AnswerKey

Frontend:
- Angular Reactive Forms
- Select para tipo de questao
- Campos dinamicos conforme o tipo
- Angular Service com HttpClient

Cuidados
Nao trate todo gabarito como texto simples.
Questao numerica precisa de tolerancia.
Questao de texto curto precisa de normalizacao.
Questao de multipla escolha precisa comparar alternativas.

Regra importante
O gabarito estruturado e o que diferencia esse projeto de um sistema fraco.

