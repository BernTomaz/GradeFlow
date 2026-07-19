GradeFlow - Etapa 03 - Banco de Dados e EF Core

Objetivo
Persistir as entidades principais no banco de dados.

Tecnologias
- Entity Framework Core
- SQL Server
- Migrations

Banco recomendado
Se você usa Visual Studio e quer menos atrito, comece com SQL Server.
Se quiser padronizar o projeto, mantenha SQL Server do início ao fim.

Tarefas
- Instalar pacotes do EF Core
- Criar GradeFlowDbContext
- Criar DbSet das entidades
- Configurar relacionamentos
- Configurar propriedades obrigatórias
- Criar primeira migration
- Aplicar migration no banco

Relacionamentos principais
Assignment 1:N Question
Question 1:1 AnswerKey
Assignment 1:N Submission
Submission 1:N StudentAnswer
StudentAnswer 1:1 CorrectionResult

Pacotes comuns para SQL Server
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design

Ponto de atenção
Não deixe connection string real exposta no GitHub.
Use appsettings.Development.json local ou user secrets.
