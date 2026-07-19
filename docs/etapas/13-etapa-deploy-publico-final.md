# GradeFlow - Etapa 13 - Deploy público Final

## Objetivo

Publicar o GradeFlow em ambiente público somente quando as funcionalidades principais e documentação estiverem concluídas.

Esta etapa fecha o projeto como demonstração profissional de portfolio.

---

## Pre-requisitos

Antes desta etapa, devem estar concluídas:

- CI validando backend e frontend;
- configurações por ambiente;
- endpoint `/health`;
- Docker local;
- estratégia controlada de migrations;
- funcionalidades planejadas antes do deploy;
- decisão de hospedagem e custo.

---

## Decisão de Hospedagem

Escolher apenas uma arquitetura de deploy.

Possibilidades:

### Backend

- Azure App Service;
- Render;
- Railway;
- servico de containers compativel.

### Frontend

- Azure Static Web Apps;
- Vercel;
- Netlify;
- container Nginx.

### Banco

- Azure SQL;
- SQL Server gerenciado compativel.

A escolha deve considerar:

- custo;
- plano gratuito;
- suspensao em plano gratuito;
- suporte a Docker;
- HTTPS;
- variáveis de ambiente;
- integracao GitHub;
- banco persistente;
- facilidade de demonstração.

---

## CD e Deploy

Arquivo esperado:

```text
.github/workflows/deploy.yml
```

Executar:

- apenas na `main`;
- somente após CI aprovado;
- com environment protegido, se disponivel.

Possiveis etapas:

- build de imagens;
- publicacao no GHCR, se a hospedagem consumir imagens;
- aplicação controlada de migrations;
- deploy backend;
- deploy frontend;
- smoke test pos-deploy.

---

## Secrets

Usar:

- GitHub Actions Secrets;
- GitHub Environments;
- secrets do provedor.

Nunca versionar ou imprimir:

- JWT real;
- connection string real;
- senha do banco;
- tokens;
- credenciais do provedor.

---

## Validação Pública

Validar:

- `/health`;
- Swagger, se exposto;
- registro;
- login;
- refresh atual;
- rota protegida;
- `401`;
- `403`;
- ownership;
- criação de avaliação;
- criação de questão;
- submissão;
- correção;
- revisão manual;
- auditoria.

Frontend:

- carregamento inicial;
- rotas Angular;
- refresh de pagina;
- login/logout;
- navegacao protegida;
- chamadas para API pública;
- mensagens de erro;
- ausencia de mixed content;
- HTTPS valido.

Observabilidade mínima:

- logs de startup;
- logs de erro;
- health check;
- forma de consultar falhas no provedor.

---

## README

Adicionar:

- URL pública;
- arquitetura de hospedagem;
- instruções de demonstração;
- usuário demo, somente se criado de forma segura;
- limitacoes conhecidas;
- status do CI.

---

## Critérios de Aceite

- CD realiza deploy automatizado.
- Backend está publicado com HTTPS.
- Frontend está publicado com HTTPS.
- Frontend usa URL pública da API.
- CORS permite apenas origens esperadas.
- Banco é persistente.
- Migrations seguem estratégia controlada.
- Secrets não aparecem no repositório nem nos logs.
- Rollback está documentado.
- README possui links e instruções atualizadas.

## Commit sugerido

```text
ci: add automated production deployment workflow
```
