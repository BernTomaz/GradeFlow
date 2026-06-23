GradeFlow - Etapa 03 - Banco de Dados e EF Core

Objetivo
Persistir as entidades principais no banco de dados.

Tecnologias
- Entity Framework Core
- SQL Server ou PostgreSQL
- Migrations

Banco recomendado
Se voce usa Visual Studio e quer menos atrito, comece com SQL Server.
Se quiser algo mais portavel e comum em deploy, PostgreSQL tambem e excelente.

Tarefas
- Instalar pacotes do EF Core
- Criar GradeFlowDbContext
- Criar DbSet das entidades
- Configurar relacionamentos
- Configurar propriedades obrigatorias
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

Pacotes comuns para PostgreSQL
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.Design

Ponto de atencao
Nao deixe connection string real exposta no GitHub.
Use appsettings.Development.json local ou user secrets.

