# Status do Projeto

Projeto em desenvolvimento ativo.

O MVP principal já possui backend, frontend Angular, dashboard inicial, assistente Flow para ajuda contextual, correção automática, revisão manual, auditoria, testes automatizados, autenticação baseada em perfis, importação CSV, relatórios e exportações.

## Etapa Atual

- Etapas 01 a 12 concluídas.
- Etapa 12 concluída: fechamento para demonstração.
- Próximo trabalho operacional: Etapa 13, deploy público final.
- Recursos futuros ficam organizados na Etapa 14.

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
- Senha forte obrigatória na configuração inicial, criação de usuário pelo Admin e alteração de senha
- Cadastro público fechado; novos usuários são criados pelo Admin
- Limite temporário de tentativas inválidas de login
- Controle de acesso baseado em perfis:
  - Admin
  - Teacher
  - Student
- Autorização por proprietário da avaliação
- Swagger/OpenAPI
- CORS configurado por ambiente
- Health check em `/health`
- Testes automatizados
- Importação CSV de submissões
- Relatório de desempenho por avaliação
- Exportação de resultados em CSV, Excel e PDF

### Frontend

- Angular 20
- Standalone Components
- TypeScript
- Reactive Forms
- HttpClient
- Route Guards
- HTTP Interceptors
- Dashboard inicial após login
- Indicadores por avaliação: questões, alunos avaliados, média e respostas pendentes de revisão
- Gráfico de acertos por questão com visualização em barras, colunas ou pizza
- Página própria de relatório por avaliação
- Exportação do dashboard em PDF via impressão do navegador
- Login, configuração inicial do primeiro Admin e criação controlada de usuários
- Alteração de senha autenticada
- Alteração de nome autenticada
- Redefinição de senha temporária pelo Admin
- CRUD de avaliações
- CRUD de questões
- CRUD de submissões
- Visualização de resultados de correção
- Importação de submissões via CSV
- Exportação de relatórios em CSV, Excel e PDF
- Assistente Flow com mascote e ajuda contextual sobre avaliações, gabaritos, respostas, correção, revisão e relatórios
- Layout administrativo responsivo com sidebar recolhível
- Temas claro, escuro e conforme o sistema

### DevOps e Operação

- Pipeline CI com GitHub Actions
- Docker e Docker Compose para execução local
- Health checks da API e do banco
- Estratégia controlada de migrations com script idempotente
- SQL Server do Docker sem porta publicada para o host
- Script local de backup do banco Docker

## Próximos Passos

- Publicar o projeto em ambiente público.
- Evoluir recursos futuros apenas depois do MVP demonstrável e publicado.

## Tipos de Questão Suportados

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização

## Ajustes de auditoria e acesso (06/11/2026)

Validação automatizada concluída conforme resultados do terminal fornecidos pelo usuário: 92 testes backend e 21 frontend aprovados, sem falhas. Builds Docker concluídos, API e SQL Server saudáveis e frontend iniciado. Detalhes em [testes](testes.md).

Correções automáticas e revisões manuais registram o usuário executor em `ReviewedByUserId`; recorreções acrescentam histórico em `CorrectionLog`. Não há reconstrução do histórico antigo nem migration. Aluno lê questões sem gabarito e opera somente suas submissões. O catálogo ainda não possui distribuição por turma. A validação completa pela interface do Aluno e o deploy público permanecem pendentes.
