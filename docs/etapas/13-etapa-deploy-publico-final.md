# GradeFlow - Etapa 13 - Deploy Publico Final

## Objetivo

Publicar o GradeFlow em ambiente publico somente quando as funcionalidades principais e documentacao estiverem concluidas.

Esta etapa fecha o projeto como demonstracao profissional de portfolio.

---

## Pre-requisitos

Antes desta etapa, devem estar concluidas:

- CI validando backend e frontend;
- configuracoes por ambiente;
- endpoint `/health`;
- Docker local;
- estrategia controlada de migrations;
- funcionalidades planejadas antes do deploy;
- decisao de hospedagem e custo.

---

## Decisao de hospedagem

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
- variaveis de ambiente;
- integracao GitHub;
- banco persistente;
- facilidade de demonstracao.

---

## CD e Deploy

Arquivo esperado:

```text
.github/workflows/deploy.yml
```

Executar:

- apenas na `main`;
- somente apos CI aprovado;
- com environment protegido, se disponivel.

Possiveis etapas:

- build de imagens;
- publicacao no GHCR, se a hospedagem consumir imagens;
- aplicacao controlada de migrations;
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

## Validacao publica

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
- criacao de avaliacao;
- criacao de questao;
- submissao;
- correcao;
- revisao manual;
- auditoria.

Frontend:

- carregamento inicial;
- rotas Angular;
- refresh de pagina;
- login/logout;
- navegacao protegida;
- chamadas para API publica;
- mensagens de erro;
- ausencia de mixed content;
- HTTPS valido.

Observabilidade minima:

- logs de startup;
- logs de erro;
- health check;
- forma de consultar falhas no provedor.

---

## README

Adicionar:

- URL publica;
- arquitetura de hospedagem;
- instrucoes de demonstracao;
- usuario demo, somente se criado de forma segura;
- limitacoes conhecidas;
- status do CI.

---

## Criterios de aceite

- CD realiza deploy automatizado.
- Backend esta publicado com HTTPS.
- Frontend esta publicado com HTTPS.
- Frontend usa URL publica da API.
- CORS permite apenas origens esperadas.
- Banco e persistente.
- Migrations seguem estrategia controlada.
- Secrets nao aparecem no repositorio nem nos logs.
- Rollback esta documentado.
- README possui links e instrucoes atualizadas.

## Commit sugerido

```text
ci: add automated production deployment workflow
```
