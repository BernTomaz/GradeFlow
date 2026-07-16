GradeFlow - Etapa 00 - Visao Geral

Objetivo do projeto
Criar um sistema web para cadastro de avaliacoes, questoes, gabaritos estruturados, respostas de alunos, correcao automatica e revisao manual de notas.

Problema que o sistema resolve
Professores e avaliadores precisam corrigir atividades de forma mais organizada, rastreavel e menos manual, principalmente quando existem criterios objetivos de correcao.

Ideia central
O projeto nao deve ser apenas um comparador de texto.
O ponto principal deve ser um motor de correcao capaz de aplicar regras diferentes para cada tipo de questao.

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
- Strategy Pattern para o motor de correcao
- Controllers finos
- Services para regras de aplicacao
- Domain para entidades e regras centrais

Ordem recomendada
Segue a ordem oficial de implementacao.
1. MVP backend
2. Motor de correcao
3. Frontend simples
4. Testes
5. Revisao manual
6. Login
7. Preparacao DevOps local
8. Importacao e relatorios
9. Deploy publico final
10. IA, OCR e recursos avancados

Ponto de atencao
Nao comece por IA, OCR, dashboard ou login complexo.
Primeiro faca o sistema corrigir uma avaliacao simples de ponta a ponta.
