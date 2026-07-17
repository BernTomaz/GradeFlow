# Integração Contínua

[![CI](https://github.com/BernTomaz/GradeFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/BernTomaz/GradeFlow/actions/workflows/ci.yml)

O workflow `CI` valida backend e frontend em todo push para `main` e em pull requests direcionados para `main`.

Ele restaura, compila e testa a solução .NET, instala as dependências do Angular com `npm ci`, compila o frontend e executa os testes.

As execuções ficam disponíveis na aba [Actions](https://github.com/BernTomaz/GradeFlow/actions).
