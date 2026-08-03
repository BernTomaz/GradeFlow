IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [Assignments] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Subject] nvarchar(200) NULL,
        [TotalPoints] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [Questions] (
        [Id] uniqueidentifier NOT NULL,
        [AssignmentId] uniqueidentifier NOT NULL,
        [Text] nvarchar(4000) NOT NULL,
        [Type] int NOT NULL,
        [Points] decimal(18,2) NOT NULL,
        [Order] int NOT NULL,
        [CorrectionConfigJson] nvarchar(4000) NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Questions_Assignments_AssignmentId] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [Submissions] (
        [Id] uniqueidentifier NOT NULL,
        [AssignmentId] uniqueidentifier NOT NULL,
        [StudentName] nvarchar(200) NOT NULL,
        [StudentEmail] nvarchar(320) NULL,
        [Status] int NOT NULL,
        [FinalScore] decimal(18,2) NOT NULL,
        [SubmittedAt] datetime2 NOT NULL,
        [CorrectedAt] datetime2 NULL,
        [ReviewedAt] datetime2 NULL,
        CONSTRAINT [PK_Submissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Submissions_Assignments_AssignmentId] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [AnswerKeys] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [CorrectAnswer] nvarchar(2000) NOT NULL,
        [AcceptedAnswersJson] nvarchar(4000) NULL,
        [KeywordsJson] nvarchar(4000) NULL,
        [Tolerance] decimal(18,4) NULL,
        [FeedbackCorrect] nvarchar(2000) NULL,
        [FeedbackIncorrect] nvarchar(2000) NULL,
        CONSTRAINT [PK_AnswerKeys] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnswerKeys_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentAnswers] (
        [Id] uniqueidentifier NOT NULL,
        [SubmissionId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Answer] nvarchar(4000) NULL,
        [ScoreAwarded] decimal(18,2) NOT NULL,
        [IsCorrect] bit NOT NULL,
        [Feedback] nvarchar(2000) NULL,
        [NeedsReview] bit NOT NULL,
        CONSTRAINT [PK_StudentAnswers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentAnswers_Submissions_SubmissionId] FOREIGN KEY ([SubmissionId]) REFERENCES [Submissions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE TABLE [CorrectionResults] (
        [Id] uniqueidentifier NOT NULL,
        [StudentAnswerId] uniqueidentifier NOT NULL,
        [IsCorrect] bit NOT NULL,
        [ScoreAwarded] decimal(18,2) NOT NULL,
        [Feedback] nvarchar(2000) NULL,
        [CorrectionType] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CorrectionResults] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CorrectionResults_StudentAnswers_StudentAnswerId] FOREIGN KEY ([StudentAnswerId]) REFERENCES [StudentAnswers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AnswerKeys_QuestionId] ON [AnswerKeys] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CorrectionResults_StudentAnswerId] ON [CorrectionResults] ([StudentAnswerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Questions_AssignmentId] ON [Questions] ([AssignmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentAnswers_QuestionId] ON [StudentAnswers] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentAnswers_SubmissionId] ON [StudentAnswers] ([SubmissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Submissions_AssignmentId] ON [Submissions] ([AssignmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260625193322_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260625193322_InitialCreate', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260629130900_RequireStudentAnswerAnswer'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentAnswers]') AND [c].[name] = N'Answer');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [StudentAnswers] DROP CONSTRAINT ' + @var + ';');
    EXEC(N'UPDATE [StudentAnswers] SET [Answer] = N'''' WHERE [Answer] IS NULL');
    ALTER TABLE [StudentAnswers] ALTER COLUMN [Answer] nvarchar(4000) NOT NULL;
    ALTER TABLE [StudentAnswers] ADD DEFAULT N'' FOR [Answer];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260629130900_RequireStudentAnswerAnswer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260629130900_RequireStudentAnswerAnswer', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707141941_AddCorrectionLogs'
)
BEGIN
    CREATE TABLE [CorrectionLogs] (
        [Id] uniqueidentifier NOT NULL,
        [SubmissionId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [CorrectionType] nvarchar(100) NOT NULL,
        [OriginalAnswer] nvarchar(4000) NOT NULL,
        [NormalizedAnswer] nvarchar(4000) NULL,
        [ExpectedAnswer] nvarchar(2000) NULL,
        [Score] decimal(18,2) NOT NULL,
        [Message] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ReviewedByUserId] nvarchar(200) NULL,
        CONSTRAINT [PK_CorrectionLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707141941_AddCorrectionLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260707141941_AddCorrectionLogs', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708162000_AddUsers'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Email] nvarchar(320) NOT NULL,
        [PasswordHash] nvarchar(1000) NOT NULL,
        [Role] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708162000_AddUsers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708162000_AddUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708162000_AddUsers', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    ALTER TABLE [Assignments] ADD [TeacherUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    ALTER TABLE [Submissions] ADD [StudentUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    CREATE INDEX [IX_Assignments_TeacherUserId] ON [Assignments] ([TeacherUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    CREATE INDEX [IX_Submissions_StudentUserId] ON [Submissions] ([StudentUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    ALTER TABLE [Assignments] ADD CONSTRAINT [FK_Assignments_Users_TeacherUserId] FOREIGN KEY ([TeacherUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    ALTER TABLE [Submissions] ADD CONSTRAINT [FK_Submissions_Users_StudentUserId] FOREIGN KEY ([StudentUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260708170000_AddUserOwnership'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260708170000_AddUserOwnership', N'10.0.0');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803173508_AddSubmissionDeletedAt'
)
BEGIN
    ALTER TABLE [Submissions] ADD [DeletedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803173508_AddSubmissionDeletedAt'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260803173508_AddSubmissionDeletedAt', N'10.0.0');
END;

COMMIT;
GO

