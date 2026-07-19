GradeFlow - Fluxo 07 - Autenticação e Permissões

Objetivo
Controlar o acesso ao sistema por perfil de usuário.

Quando implementar
Depois do MVP manual funcionar.
Não deve ser a primeira etapa.

Perfis
- Admin
- Teacher
- Student

Fluxo de login
1. usuário informa email e senha.
2. Backend valida credenciais.
3. Backend gera JWT.
4. Frontend armazena token de forma controlada.
5. Angular envia token nas requisicoes.
6. Backend valida token e role.

Entidade principal
User:
- Id
- Name
- Email
- PasswordHash
- Role
- CreatedAt
- UpdatedAt

Endpoints recomendados
POST /api/auth/register
POST /api/auth/login
POST /api/auth/change-password
POST /api/auth/refresh-token

DTOs sugeridos
RegisterRequest:
- Name
- Email
- Password
- Role

LoginRequest:
- Email
- Password

AuthResponse:
- Token
- User

Tecnologias envolvidas
Backend:
- ASP.NET Core Identity ou autenticação JWT manual
- BCrypt ou PasswordHasher
- JWT Bearer Authentication
- Authorization por roles

Banco:
- EF Core
- Tabela Users
- Tabela RefreshTokens, em etapa posterior

Frontend:
- Angular Guards
- Angular Interceptors
- AuthService
- Rotas protegidas

Regras de permissão
Admin:
- Pode ver tudo.

Teacher:
- Pode gerenciar apenas suas avaliações.

Student:
- Pode ver apenas suas próprias submissões.

Observação
O endpoint atual `refresh-token` reemite access token.
Refresh token persistente, revogável e rotacionado fica para etapa futura.

Cuidados
- Nunca salvar senha pura.
- Nunca confiar apenas no frontend.
- Validar permissão no backend.
- Não expor PasswordHash em responses.

Regra importante
Login é importante, mas não prova o valor do projeto.
O valor é provado pelo motor de correção funcionando.
