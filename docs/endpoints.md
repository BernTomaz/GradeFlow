# Endpoints Principais

| Recurso | Endpoints |
|----------|----------|
| Assignments | GET, POST `/api/assignments` |
| Assignment por Id | GET, PUT, DELETE `/api/assignments/{id}` |
| Questions | GET, POST `/api/assignments/{assignmentId}/questions` |
| Question por Id | GET, PUT, DELETE `/api/questions/{id}` |
| Submissions | GET, POST `/api/assignments/{assignmentId}/submissions` |
| Submission por Id | GET, PUT, DELETE `/api/submissions/{id}` |
| Correção | POST `/api/submissions/{id}/correct` |
| Correção de Questão | POST `/api/submissions/{id}/questions/{questionId}/correct` |
| Revisão Manual | PUT `/api/student-answers/{answerId}/review` |
| Auditoria | GET `/api/submissions/{id}/correction-logs` |
| Auth | POST `/api/auth/register` |
| Login | POST `/api/auth/login` |
| Alteração de senha | POST `/api/auth/change-password` |
| Reemissão de token | POST `/api/auth/refresh-token` |

Rotas protegidas retornam `401` quando o token está ausente, inválido ou expirado, e `403` quando o usuário autenticado não possui o perfil exigido.
