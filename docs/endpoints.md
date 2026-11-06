# Endpoints Principais

| Recurso | Endpoints |
|----------|----------|
| Assignments | GET, POST `/api/assignments` |
| Assignment por Id | GET, PUT, DELETE `/api/assignments/{id}` |
| Questions | GET, POST `/api/assignments/{assignmentId}/questions` |
| Question por Id | GET, PUT, DELETE `/api/questions/{id}` |
| Submissions | GET, POST `/api/assignments/{assignmentId}/submissions` |
| Importação CSV de submissões | POST `/api/assignments/{assignmentId}/submissions/import` |
| Relatório da avaliação | GET `/api/assignments/{assignmentId}/report` |
| Exportação CSV | GET `/api/assignments/{assignmentId}/export/csv` |
| Exportação Excel | GET `/api/assignments/{assignmentId}/export/excel` |
| Exportação PDF | GET `/api/assignments/{assignmentId}/export/pdf` |
| Submission por Id | GET, PUT, DELETE `/api/submissions/{id}` |
| Correção | POST `/api/submissions/{id}/correct` |
| Correção de Questão | POST `/api/submissions/{id}/questions/{questionId}/correct` |
| Revisão Manual | PUT `/api/student-answers/{answerId}/review` |
| Auditoria | GET `/api/submissions/{id}/correction-logs` |
| Status da configuração inicial | GET `/api/setup/status` |
| Criação do primeiro Admin | POST `/api/setup/admin` |
| Criação de usuário pelo Admin | POST `/api/auth/register` |
| Listagem de usuários (Admin) | GET `/api/auth/users` |
| Redefinição de senha (Admin) | POST `/api/auth/users/{id}/reset-password` |
| Resposta individual própria (Aluno) ou gerenciada (Admin/Professor) | PUT `/api/submissions/{submissionId}/questions/{questionId}/answer` |
| Dados do aluno na submissão | PUT `/api/submissions/{id}/student` |
| Login | POST `/api/auth/login` |
| Alteração de senha | POST `/api/auth/change-password` |
| Alteração de nome | POST `/api/auth/change-name` |
| Reemissão de token | POST `/api/auth/refresh-token` |

Rotas protegidas retornam `401` quando o token está ausente, inválido ou expirado, e `403` quando o usuário autenticado não possui o perfil exigido.

Tokens também retornam `401` após mudança de senha, redefinição, remoção do usuário ou mudança de perfil. Tokens anteriores à introdução da validação de sessão precisam ser substituídos por um novo login.

Quando há troca obrigatória de senha, as rotas protegidas retornam `403`, exceto `POST /api/auth/change-password`. A alteração de senha retorna `200` com `AuthResponse` (token novo, validade e usuário), substituindo o antigo retorno `204`. O frontend deve salvar a nova sessão; o token anterior fica inválido.

`DELETE /api/submissions/{id}` usa exclusão lógica: a submissão recebe `DeletedAt`, sai das listas operacionais e mantém os registros de auditoria vinculados.

## Acesso por recurso

- Aluno consulta avaliações e questões, com `answerKey: null`, e cria submissões vinculadas ao próprio usuário. Listagem e acesso às submissões continuam restritos às próprias.
- Edição integral, exclusão, importação, relatórios, correção e revisão exigem Admin ou Professor responsável. Aluno pode alterar respostas individuais e dados de identificação de sua submissão.
- Logs incluem `ReviewedByUserId` para Admin e Professor responsável. Consultar logs de uma submissão própria não expõe `ExpectedAnswer` nem o identificador do revisor ao Aluno.
- Professor não acessa avaliações sem responsável; Admin mantém acesso administrativo. O catálogo de avaliações é consultável por Alunos; ainda não há distribuição por turma ou publicação.
- Falta de perfil retorna 403; recursos sem acesso por propriedade normalmente retornam 404. A listagem de questões sem acesso retorna 200 com lista vazia.
