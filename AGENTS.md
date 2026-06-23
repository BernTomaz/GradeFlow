# AGENTS.md

## Projeto

GradeFlow é um sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

## Fonte de Verdade

Use os arquivos abaixo como guia principal do projeto:

- `README.md`
- `docs/etapas/`
- `docs/fluxos/`

## Regra Principal

Implemente o projeto por etapas.

Não implemente funcionalidades futuras antes da etapa atual estar concluída.

## Stack

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

Banco:
- SQL Server inicialmente

Testes:
- xUnit
- FluentAssertions

## Arquitetura

- Controllers devem ser finos.
- Regras de negócio devem ficar em services, domain ou no motor de correção.
- Não coloque lógica de correção dentro dos controllers.
- O motor de correção deve usar Strategy Pattern.
- Não implemente IA, OCR, upload, relatórios ou login antes do MVP.
- Sempre que possível, rode build e testes após alterações.

## Ordem Recomendada

1. `docs/etapas/01-etapa-estrutura-backend.txt`
2. `docs/etapas/02-etapa-modelagem-dominio.txt`
3. `docs/etapas/03-etapa-banco-efcore.txt`
4. `docs/etapas/04-etapa-crud-avaliacoes-questoes.txt`
5. `docs/etapas/05-etapa-submissao-respostas.txt`
6. `docs/etapas/06-etapa-motor-correcao.txt`
7. `docs/etapas/09-etapa-testes.txt`
8. `docs/etapas/07-etapa-frontend-angular.txt`
9. `docs/etapas/08-etapa-revisao-manual-auditoria.txt`