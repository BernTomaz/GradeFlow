# Revisao Tecnica de Infraestrutura

Data: 2026-07-31

## A. Banco de Dados e Migrations

- [x] `init.sql` atualiza a senha do login `gradeflow_app` com `ALTER LOGIN` quando `APP_DB_PASSWORD` muda.
- [x] As migrations usam script SQL idempotente versionado em `docker/sqlserver/gradeflow-migrations.sql`.
- [x] No Docker local, `sqlserver-init` executa as migrations antes da API.
- [x] Em producao, a aplicacao nao deve executar migrations. Use um usuario operacional autorizado a DDL, conforme `docs/operacao/migrations.md`.
- [x] `gradeflow_app` recebe apenas `db_datareader` e `db_datawriter`, sem `db_owner`.

## B. Inicializacao do Ambiente Docker

- [x] `sqlserver-init` falha se `gradeflow-migrations.sql` nao existir.
- [x] `sqlcmd -b` faz o container falhar quando `init.sql` ou migrations retornam erro.
- [x] `gradeflow-api` depende de `sqlserver-init` com `service_completed_successfully`.
- [x] `gradeflow-web` depende do health check da API.
- [x] Dockerfiles usam `context: .`, `WORKDIR` e `COPY` consistentes com a estrutura atual.
- [x] O Compose possui `restart: unless-stopped` e rede nomeada `gradeflow-network`.

## C. Configuracao do Ambiente

- [x] `.env.example` documenta as variaveis usadas pelo `docker-compose.yml`: `MSSQL_SA_PASSWORD`, `APP_DB_PASSWORD` e `JWT_KEY`.
- [x] `.env` real esta ignorado pelo Git.
- [x] Arquivos versionados usam placeholders, nao segredos reais.

## D. Pipeline de Integracao Continua

- [x] O CI restaura, compila e testa o backend.
- [x] O CI instala dependencias, compila e testa o frontend.
- [x] O build da solucao foi validado localmente.
- [x] O pipeline ja usa cache de npm e cancelamento de execucoes antigas.

## E. Documentacao Tecnica

- [x] `AGENTS.md` esta consistente com a ordem de etapas e arquitetura atual.
- [x] `docs/configuracao.md`, `docs/ci.md` e `docs/operacao/migrations.md` cobrem o fluxo atual.
- [x] Esta revisao registra os pontos de infraestrutura validados.

## F. Melhorias Opcionais

- [x] `.editorconfig` ja existe.
- [x] `restart: unless-stopped` ja esta configurado.
- [x] A rede Docker nomeada ja existe.

## Validacoes Executadas

- `docker compose config`
- `dotnet build GradeFlow.slnx -m:1`
- `dotnet test GradeFlow.slnx --no-build -m:1`
- `npm test -- --watch=false --browsers=ChromeHeadless`

Observacao: a primeira execucao local do teste frontend falhou com `spawn EPERM` por restricao ao abrir o Chrome headless no ambiente. Reexecutado com permissao para abrir o navegador, passou com 2 testes.
