# GradeFlow

Sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

[![CI](https://github.com/BernTomaz/GradeFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/BernTomaz/GradeFlow/actions)

## Documentação

A documentação detalhada está dividida por assunto, etapa, fluxo e operação.

- [Wiki do GradeFlow](https://github.com/BernTomaz/GradeFlow/wiki)
- [Visão geral do projeto](docs/etapas/00-etapa-visao-geral.md)
- [Status do projeto](docs/status-projeto.md)
- [Arquitetura](docs/arquitetura.md)
- [Configuração e execução local](docs/configuracao.md)
- [Endpoints principais](docs/endpoints.md)
- [Testes](docs/testes.md)
- [Integração contínua](docs/ci.md)
- [Roadmap](docs/roadmap.md)
- [Roadmap de implementação](docs/fluxos/10-roadmap-implementacao.md)
- [Tecnologias e arquitetura](docs/fluxos/00-visao-geral-tecnologias.md)
- [Estratégia de migrations](docs/operacao/migrations.md)

## Status

Projeto em desenvolvimento ativo.

O MVP principal já possui backend, frontend Angular, correção automática, revisão manual, auditoria, testes automatizados, autenticação baseada em perfis, importação CSV, relatórios e exportações.

Etapas 01 a 11 concluídas. Os próximos trabalhos são fechamento para demonstração, deploy público final e recursos futuros.

Detalhes completos: [status do projeto](docs/status-projeto.md).

## Processos do Sistema

- [Criar avaliação](docs/fluxos/01-fluxo-criar-avaliacao.md)
- [Criar questões e gabarito](docs/fluxos/02-fluxo-criar-questoes-gabarito.md)
- [Submeter respostas](docs/fluxos/03-fluxo-submissao-respostas.md)
- [Correção automática](docs/fluxos/04-fluxo-correcao-automatica.md)
- [Revisão manual](docs/fluxos/05-fluxo-revisao-manual.md)
- [Relatórios e exportações](docs/fluxos/06-fluxo-relatorios-exportacoes.md)
- [Autenticação e permissões](docs/fluxos/07-fluxo-autenticacao-permissoes.md)
- [Importação CSV](docs/fluxos/08-fluxo-importacao-csv-excel.md)
- [Upload, IA e OCR futuros](docs/fluxos/09-fluxo-upload-arquivos-ia-ocr-futuro.md)

## Etapas de Implementação

- [00 - Visão geral](docs/etapas/00-etapa-visao-geral.md)
- [01 - Estrutura backend](docs/etapas/01-etapa-estrutura-backend.md)
- [02 - Modelagem de domínio](docs/etapas/02-etapa-modelagem-dominio.md)
- [03 - Banco com EF Core](docs/etapas/03-etapa-banco-efcore.md)
- [04 - CRUD de avaliações e questões](docs/etapas/04-etapa-crud-avaliacoes-questoes.md)
- [05 - Submissão de respostas](docs/etapas/05-etapa-submissao-respostas.md)
- [06 - Motor de correção](docs/etapas/06-etapa-motor-correcao.md)
- [07 - Frontend Angular](docs/etapas/07-etapa-frontend-angular.md)
- [08 - Testes](docs/etapas/08-etapa-testes.md)
- [09 - Revisão manual e auditoria](docs/etapas/09-etapa-revisao-manual-auditoria.md)
- [10 - Login e permissões](docs/etapas/10-etapa-login-permissoes.md)
- [10.1 - CI, Docker e deploy final](docs/etapas/10.1-etapa-cicd-docker-deploy-final.md)
- [11 - Importação e relatórios](docs/etapas/11-etapa-importacao-relatorios.md)
- [12 - Fechamento para demonstração](docs/etapas/12-etapa-fechamento-demonstracao.md)
- [13 - Deploy público final](docs/etapas/13-etapa-deploy-publico-final.md)
- [14 - Recursos futuros](docs/etapas/14-etapa-recursos-futuros.md)

## Stack

Backend:

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication

Frontend:

- Angular 20
- TypeScript
- Reactive Forms
- HttpClient
- PrimeNG Aura
- PrimeIcons

Testes:

- xUnit
- FluentAssertions

## Configuração Rápida

Guia completo: [configuração e execução local](docs/configuracao.md).

Pré-requisitos:

- .NET 10 SDK
- Node.js 20.19 ou superior
- Angular CLI
- SQL Server LocalDB, Express ou Developer
- EF Core Tools

Restaurar o backend:

```powershell
dotnet restore
```

Instalar o frontend:

```powershell
cd src/GradeFlow.Web
npm install
```

Configurar a chave JWT local:

```powershell
dotnet user-secrets set "Jwt:Key" "troque-por-uma-chave-local-com-32-ou-mais-caracteres" --project src\GradeFlow.Api
```

Aplicar migrations em desenvolvimento local:

```powershell
dotnet ef database update --project src\GradeFlow.Infrastructure --startup-project src\GradeFlow.Api
```

Executar a API:

```powershell
dotnet run --project src\GradeFlow.Api --launch-profile https
```

Executar o frontend:

```powershell
cd src\GradeFlow.Web
npm start
```

Endereços locais:

- API: `https://localhost:7013`
- Swagger: `https://localhost:7013/swagger`
- Health check: `https://localhost:7013/health`
- Frontend: `http://localhost:4200`

## Docker Compose

Crie um `.env` local a partir do `.env.example` e ajuste `MSSQL_SA_PASSWORD` e `JWT_KEY`.

```powershell
docker compose build
docker compose up
```

Serviços locais:

- Frontend: `http://localhost:4200`
- API: `http://localhost:8080`
- Health check: `http://localhost:8080/health`
- SQL Server: `localhost,1433`

As migrations não são aplicadas automaticamente pelo Compose. Veja [estratégia de migrations](docs/operacao/migrations.md).

## Referências Rápidas

- [Arquitetura do projeto](docs/arquitetura.md)
- [Endpoints principais](docs/endpoints.md)
- [Guia completo de testes](docs/testes.md)
- [Integração contínua](docs/ci.md)
- [Roadmap](docs/roadmap.md)

## Testes

Guia completo: [guia de testes](docs/testes.md).

Backend:

```powershell
dotnet clean GradeFlow.slnx -m:1
dotnet restore GradeFlow.slnx
dotnet build GradeFlow.slnx --no-restore -m:1
dotnet test GradeFlow.slnx --no-build -m:1
```

Frontend:

```powershell
cd src\GradeFlow.Web
npm ci
npm run build
npm test -- --watch=false --browsers=ChromeHeadless
```

## Licença

Este projeto é disponibilizado para fins educacionais, estudo e demonstração de portfólio profissional.
