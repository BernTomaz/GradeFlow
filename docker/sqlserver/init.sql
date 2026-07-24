IF DB_ID(N'GradeFlow') IS NULL
BEGIN
    CREATE DATABASE GradeFlow;
END

IF SUSER_ID(N'gradeflow_app') IS NULL
BEGIN
    CREATE LOGIN gradeflow_app WITH PASSWORD = N'$(APP_DB_PASSWORD)';
END

USE GradeFlow;

IF USER_ID(N'gradeflow_app') IS NULL
BEGIN
    CREATE USER gradeflow_app FOR LOGIN gradeflow_app;
END

IF IS_ROLEMEMBER(N'db_datareader', N'gradeflow_app') = 0
BEGIN
    ALTER ROLE db_datareader ADD MEMBER gradeflow_app;
END

IF IS_ROLEMEMBER(N'db_datawriter', N'gradeflow_app') = 0
BEGIN
    ALTER ROLE db_datawriter ADD MEMBER gradeflow_app;
END
