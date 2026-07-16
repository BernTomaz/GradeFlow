# Estrategia de Migrations

## Decisao

O GradeFlow usa **script SQL idempotente aplicado de forma controlada** antes da atualizacao da aplicacao.

Nao usar `Database.Migrate()` no startup em producao. Em ambientes com mais de uma instancia, migration no startup pode executar concorrentemente e deixar o deploy imprevisivel.

## Gerar script idempotente

Na raiz do repositorio:

```powershell
.\scripts\database\generate-migration-script.ps1
```

Saida padrao:

```txt
artifacts/database/gradeflow-migrations.sql
```

O diretorio `artifacts/` e ignorado pelo Git.

## Aplicar em ambiente local Docker

Com o SQL Server do `docker-compose.yml` em execucao:

```powershell
docker cp artifacts/database/gradeflow-migrations.sql gradeflow-sqlserver-1:/tmp/gradeflow-migrations.sql
docker exec gradeflow-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$env:MSSQL_SA_PASSWORD" -C -d GradeFlow -i /tmp/gradeflow-migrations.sql
```

O mesmo script pode ser executado novamente. Como e idempotente, migrations ja aplicadas sao ignoradas.

## Producao

Fluxo recomendado:

1. Fazer backup do banco.
2. Gerar o script idempotente a partir do commit que sera publicado.
3. Revisar o SQL gerado.
4. Aplicar o script uma unica vez, antes do deploy da aplicacao.
5. Interromper o deploy se a migration falhar.
6. Publicar a aplicacao somente apos a migration concluir.

## Rollback

Rollback de schema nao deve ser automatico. Em caso de falha:

1. Parar o deploy da aplicacao.
2. Restaurar backup quando a alteracao for destrutiva ou incompatibilidade impedir operacao.
3. Corrigir a migration em novo commit.
4. Gerar novo script idempotente e repetir o processo.
