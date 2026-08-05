# Integração Contínua

[![CI](https://github.com/BernTomaz/GradeFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/BernTomaz/GradeFlow/actions/workflows/ci.yml)

O workflow `CI` valida backend e frontend em todo push para `main` e em pull requests direcionados para `main`.

Ele restaura, compila e testa a solução .NET, instala as dependências do Angular com `npm ci`, compila o frontend e executa os testes.

As execuções ficam disponíveis na aba [Actions](https://github.com/BernTomaz/GradeFlow/actions).

## Deploy

O workflow de deploy ainda depende da escolha de hospedagem da Etapa 13.

Antes de criar `.github/workflows/deploy.yml`, defina onde ficam backend, frontend e banco. Depois use o [checklist de deploy publico](operacao/deploy-checklist.md) e o [rollback de producao](operacao/rollback.md) como criterios minimos.
