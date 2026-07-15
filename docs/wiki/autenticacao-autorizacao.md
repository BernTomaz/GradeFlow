# Autenticacao e Autorizacao

## JWT

O GradeFlow usa JWT para autenticar usuarios e proteger rotas da API.

## Fluxo

1. Usuario realiza login.
2. Sistema gera token JWT.
3. Frontend armazena o token.
4. Frontend envia o token nas requisicoes protegidas.
5. API valida autenticacao e permissoes.

## Perfis

- Admin
- Teacher
- Student

Rotas protegidas retornam `401` quando o token esta ausente, invalido ou expirado, e `403` quando o usuario autenticado nao possui permissao.
