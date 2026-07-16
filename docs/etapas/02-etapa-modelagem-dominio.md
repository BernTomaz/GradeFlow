GradeFlow - Etapa 02 - Modelagem do Dominio

Objetivo
Criar as entidades principais do MVP.

Tecnologias
- C#
- .NET
- Entity Framework Core em etapa posterior

Entidades do MVP
- Assignment
- Question
- AnswerKey
- Submission
- StudentAnswer
- CorrectionResult

Assignment
- Id
- Title
- Description
- Subject
- TotalPoints
- Status
- CreatedAt
- UpdatedAt

Question
- Id
- AssignmentId
- Text
- Type
- Points
- Order
- CorrectionConfigJson

AnswerKey
- Id
- QuestionId
- CorrectAnswer
- AcceptedAnswersJson
- KeywordsJson
- Tolerance
- FeedbackCorrect
- FeedbackIncorrect

Submission
- Id
- AssignmentId
- StudentName
- StudentEmail
- Status
- FinalScore
- SubmittedAt
- CorrectedAt
- ReviewedAt

StudentAnswer
- Id
- SubmissionId
- QuestionId
- Answer
- ScoreAwarded
- IsCorrect
- Feedback
- NeedsReview

CorrectionResult
- Id
- StudentAnswerId
- IsCorrect
- ScoreAwarded
- Feedback
- CorrectionType
- CreatedAt

Enums recomendados
QuestionType:
- MultipleChoice
- TrueFalse
- Numeric
- ShortText

AssignmentStatus:
- Draft
- Open
- Closed

SubmissionStatus:
- Pending
- Corrected
- Reviewed

Ponto de atencao
Nao inclua User, Course, Classroom e Student no MVP inicial.
Essas entidades entram melhor depois que a correcao estiver funcionando.
User foi adicionado posteriormente na etapa de login e permissoes.
