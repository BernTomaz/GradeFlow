GradeFlow - Etapa 10 - Login e permissões

Objetivo
Controlar acesso ao sistema por usuário e perfil depois que o MVP estiver funcionando.

Quando implementar
Depois que o MVP de correção estiver funcionando.

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
POST /api/auth/change-password
POST /api/auth/refresh-token

Regras
Admin:
- Pode ver tudo.

Teacher:
- Pode gerenciar suas avaliações.

Student:
- Pode ver suas próprias submissões.

Tarefas
- Criar entidade User
- Criar migration
- Criar registro
- Criar login
- Criar alteração de senha autenticada
- Gerar JWT
- Proteger endpoints
- Criar guards no Angular
- Criar interceptor para enviar token

Observação
O endpoint atual `refresh-token` reemite um access token, mas ainda não implementa refresh token persistente, revogável e rotacionado.
Refresh token completo fica como melhoria futura.

Ponto de atenção
Nunca salve senha pura.
Nunca confie apenas na permissão do frontend.
Permissão real precisa ser validada no backend.
