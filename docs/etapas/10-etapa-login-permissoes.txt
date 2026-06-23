GradeFlow - Etapa 10 - Login e Permissoes

Objetivo
Controlar acesso ao sistema por usuario e perfil.

Quando implementar
Depois que o MVP de correcao estiver funcionando.

Tecnologias
- ASP.NET Core Authentication
- JWT Bearer
- PasswordHasher ou BCrypt
- Angular Guards
- Angular Interceptors

Perfis
- Admin
- Teacher
- Student

Entidade User
- Id
- Name
- Email
- PasswordHash
- Role
- CreatedAt
- UpdatedAt

Endpoints
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token

Regras
Admin:
- Pode ver tudo.

Teacher:
- Pode gerenciar suas avaliacoes.

Student:
- Pode ver suas proprias submisssoes.

Tarefas
- Criar entidade User
- Criar migration
- Criar registro
- Criar login
- Gerar JWT
- Proteger endpoints
- Criar guards no Angular
- Criar interceptor para enviar token

Ponto de atencao
Nunca salve senha pura.
Nunca confie apenas na permissao do frontend.
Permissao real precisa ser validada no backend.

