# Rollback de Producao

Rollback deve ser simples e manualmente controlado.

## Aplicacao

1. Pausar novo deploy.
2. Reativar a versao anterior do backend no provedor.
3. Reativar a versao anterior do frontend no provedor.
4. Validar `/health`, login e uma rota protegida.
5. Registrar o commit problemático e abrir correcao em novo commit.

## Banco

Nao fazer rollback automatico de schema.

Se a migration falhar antes da aplicacao nova entrar no ar:

1. Interromper o deploy.
2. Manter a versao atual da aplicacao.
3. Corrigir a migration em novo commit.
4. Gerar novo script idempotente.

Se a migration aplicada causar incompatibilidade:

1. Tirar a aplicacao nova do ar.
2. Restaurar backup quando a alteracao for destrutiva.
3. Publicar uma correcao compatível com o schema atual quando restaurar backup nao for necessario.

## Regra pratica

Migration destrutiva exige backup validado e janela de manutencao. Para o MVP publico, prefira migrations aditivas.

