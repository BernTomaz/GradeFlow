# API

## Visao Geral

A API expoe endpoints REST para avaliacoes, questoes, submissoes, correcao, revisao manual, auditoria e autenticacao.

## Endpoints Principais

| Recurso | Endpoints |
| --- | --- |
| Assignments | GET, POST `/api/assignments` |
| Assignment por Id | GET, PUT, DELETE `/api/assignments/{id}` |
| Questions | GET, POST `/api/assignments/{assignmentId}/questions` |
| Question por Id | GET, PUT, DELETE `/api/questions/{id}` |
| Submissions | GET, POST `/api/assignments/{assignmentId}/submissions` |
| Submission por Id | GET, PUT, DELETE `/api/submissions/{id}` |
| Correcao | POST `/api/submissions/{id}/correct` |
| Correcao de Questao | POST `/api/submissions/{id}/questions/{questionId}/correct` |
| Revisao Manual | PUT `/api/student-answers/{answerId}/review` |
| Auditoria | GET `/api/submissions/{id}/correction-logs` |
| Cadastro | POST `/api/auth/register` |
| Login | POST `/api/auth/login` |
| Alteracao de senha | POST `/api/auth/change-password` |
| Reemissao de token | POST `/api/auth/refresh-token` |
