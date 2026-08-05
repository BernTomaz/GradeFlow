# Checklist de Deploy Publico

Use este checklist antes de iniciar a Etapa 13 e a cada publicacao em producao.

## Decisoes

- [ ] Provedor do backend definido.
- [ ] Provedor do frontend definido.
- [ ] Banco SQL Server persistente definido.
- [ ] Custo e suspensao do plano gratuito entendidos.
- [ ] URL publica do frontend definida.
- [ ] URL publica da API definida.

## Configuracao

- [ ] `ConnectionStrings__DefaultConnection` configurada somente no provedor.
- [ ] `Jwt__Key` configurada somente como secret.
- [ ] `Jwt__Issuer`, `Jwt__Audience` e `Jwt__ExpirationMinutes` configurados.
- [ ] `Cors__AllowedOrigins__0` aponta somente para o frontend publico.
- [ ] Nenhum `.env`, token, senha ou connection string real esta versionado.
- [ ] Frontend chama a API publica sem mixed content.

## Banco

- [ ] Backup do banco criado antes do deploy.
- [ ] `docker/sqlserver/gradeflow-migrations.sql` foi gerado a partir do commit publicado.
- [ ] Script SQL revisado.
- [ ] Migration aplicada de forma controlada antes da aplicacao.
- [ ] Usuario da aplicacao nao aplica migrations em producao.

## Deploy

- [ ] CI passou na `main`.
- [ ] Backend publicado com HTTPS.
- [ ] Frontend publicado com HTTPS.
- [ ] CD roda somente apos CI aprovado.
- [ ] Environment protegido configurado quando o provedor permitir.

## Validacao publica

- [ ] `/health` responde.
- [ ] Login funciona.
- [ ] Token atual pode ser renovado.
- [ ] Rota protegida retorna `401` sem token.
- [ ] Perfil sem permissao recebe `403`.
- [ ] Ownership impede acesso a dados de outro professor.
- [ ] Avaliacao pode ser criada.
- [ ] Questao pode ser criada.
- [ ] Submissao pode ser criada.
- [ ] Correcao automatica funciona.
- [ ] Revisao manual registra auditoria.
- [ ] Importacao CSV funciona.
- [ ] Exportacoes CSV, Excel e PDF funcionam.
- [ ] Refresh de pagina em rotas Angular funciona.
- [ ] Logout funciona.

## Observabilidade minima

- [ ] Logs de startup acessiveis no provedor.
- [ ] Logs de erro acessiveis no provedor.
- [ ] Falha de banco aparece no health check.
- [ ] Caminho de rollback foi testado ou documentado para o provedor escolhido.

