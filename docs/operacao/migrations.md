# Estratégia de Migrations

## Decisão

O GradeFlow usa **script SQL idempotente aplicado de forma controlada** antes da atualização da aplicação.

Não usar `Database.Migrate()` no startup em produção. Em ambientes com mais de uma instância, migration no startup pode executar concorrentemente e deixar o deploy imprevisível.

## Gerar script idempotente

Na raiz do repositório:

```powershell
.\scripts\database\generate-migration-script.ps1
```

Saida padrao:

```txt
docker/sqlserver/gradeflow-migrations.sql
```

Esse arquivo é versionado para permitir que clones limpos subam com Docker sem etapa manual de banco.

## Docker local

No Docker local, o serviço `sqlserver-init` aplica `docker/sqlserver/gradeflow-migrations.sql` ao iniciar o Compose.

Em clone limpo, o fluxo local é:

```powershell
docker compose up -d --build
```

Ao criar uma migration nova, gere novamente o script SQL e versione o arquivo atualizado:

```powershell
.\scripts\database\generate-migration-script.ps1
```

O mesmo script pode ser executado novamente. Como é idempotente, migrations já aplicadas são ignoradas.

Se `docker/sqlserver/gradeflow-migrations.sql` não existir, o `sqlserver-init` falha e a API não sobe.

Para aplicar manualmente com o SQL Server do `docker-compose.yml` em execução:

```powershell
docker compose cp docker/sqlserver/gradeflow-migrations.sql sqlserver:/tmp/gradeflow-migrations.sql
docker compose exec sqlserver /bin/sh -c '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d GradeFlow -i /tmp/gradeflow-migrations.sql'
```

## Backup local Docker

Antes de aplicar migrations ou atualizar a aplicação, gere um backup:

```powershell
.\scripts\database\backup-docker.ps1
```

Saida padrao:

```txt
artifacts/backups/
```

## Produção

Fluxo recomendado:

1. Fazer backup do banco.
2. Gerar o script idempotente a partir do commit que será publicado.
3. Revisar o SQL gerado.
4. Aplicar o script uma única vez, antes do deploy da aplicação, com um usuário operacional autorizado a DDL.
5. Interromper o deploy se a migration falhar.
6. Publicar a aplicação somente após a migration concluir.

O usuário da aplicação (`gradeflow_app`) não deve aplicar migrations em produção. Ele deve permanecer restrito às permissões necessárias para operar a aplicação.

Antes da Etapa 13, valide este fluxo junto com o [checklist de deploy publico](deploy-checklist.md).

## Rollback

Rollback de schema não deve ser automático. Em caso de falha:

1. Parar o deploy da aplicação.
2. Restaurar backup quando a alteração for destrutiva ou incompatibilidade impedir operação.
3. Corrigir a migration em novo commit.
4. Gerar novo script idempotente e repetir o processo.

Detalhes operacionais: [rollback de producao](rollback.md).
