GradeFlow - Etapa 00 - Visão Geral

Objetivo do projeto
Criar um sistema web para cadastro de avaliações, questões, gabaritos estruturados, respostas de alunos, correção automática e revisão manual de notas.

Problema que o sistema resolve
Professores e avaliadores precisam corrigir atividades de forma mais organizada, rastreável e menos manual, principalmente quando existem critérios objetivos de correção.

Ideia central
O projeto não deve ser apenas um comparador de texto.
O ponto principal deve ser um motor de correção capaz de aplicar regras diferentes para cada tipo de questão.

Tecnologias principais
Backend:
- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core

Frontend:
- Angular
- TypeScript
- Reactive Forms
- HttpClient

Banco de dados:
- SQL Server

Testes:
- xUnit
- FluentAssertions

Arquitetura planejada:
- Clean Architecture simples
- Strategy Pattern para o motor de correção
- Controllers finos
- Services para regras de aplicação
- Domain para entidades e regras centrais

Ordem recomendada
Segue a ordem oficial de implementação pelos arquivos da pasta `docs/etapas`.
1. Estrutura backend
2. Modelagem de domínio
3. Banco com EF Core
4. CRUD de avaliações e questões
5. Submissão de respostas
6. Motor de correção
7. Frontend Angular
8. Testes
9. Revisão manual e auditoria
10. Login e permissões
10.1. Preparação DevOps Local
11. Importação, relatórios e exportação
12. Fechamento para demonstração
13. Deploy público final
14. Recursos futuros

Ponto de atenção
Não comece por IA, OCR, dashboard ou login complexo.
Primeiro faça o sistema corrigir uma avaliação simples de ponta a ponta.
