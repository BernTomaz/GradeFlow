# Status do Projeto

Projeto em desenvolvimento ativo.

O MVP principal já possui backend, frontend Angular, correção automática, revisão manual, auditoria, testes automatizados e autenticação baseada em perfis.

## Implementado

### Domínio e Correção

- Modelagem de domínio:
  - Assignment
  - Question
  - AnswerKey
  - StudentAnswer
  - Submission
  - CorrectionResult
  - CorrectionLog
  - User
- Motor de correção automática utilizando Strategy Pattern
- Correção de múltipla escolha
- Correção de verdadeiro ou falso
- Correção numérica com tolerância configurável
- Correção de texto curto com normalização de acentos, pontuação e espaços
- Correção completa da submissão
- Recorreção de questão individual
- Registro detalhado de logs de correção
- Revisão manual de respostas específicas

### Backend

- ASP.NET Core Web API
- Arquitetura em camadas inspirada em Clean Architecture
- Serviços de aplicação:
  - AssignmentService
  - AuthService
  - CorrectionService
  - QuestionService
  - SubmissionService
- Repository Pattern
- Entity Framework Core
- SQL Server
- JWT Authentication
- Endpoint autenticado para reemissão de token
- Controle de acesso baseado em perfis:
  - Admin
  - Teacher
  - Student
- Autorização por proprietário da avaliação
- Swagger/OpenAPI
- CORS configurado por ambiente
- Health check em `/health`
- Testes automatizados

### Frontend

- Angular 20
- Standalone Components
- TypeScript
- Reactive Forms
- HttpClient
- Route Guards
- HTTP Interceptors
- Login e Cadastro
- Alteração de senha autenticada
- CRUD de avaliações
- CRUD de questões
- CRUD de submissões
- Visualização de resultados de correção

### DevOps e Operação

- Pipeline CI com GitHub Actions
- Docker e Docker Compose para execução local
- Health checks da API e do banco
- Estratégia controlada de migrations com script idempotente

## Planejado

- Importação de submissões por CSV
- Relatórios de desempenho por turma
- Exportação de notas
- Dashboard com métricas
- Deploy público para demonstração
- Cobertura ampliada de testes

## Tipos de Questão Suportados

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização
