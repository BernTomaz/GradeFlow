# GradeFlow

Sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

## Objetivo

O GradeFlow tem como objetivo auxiliar professores, avaliadores e instituições no processo de correção de atividades, provas e trabalhos.

A proposta principal é permitir que o usuário cadastre avaliações com questões estruturadas, configure gabaritos por tipo de questão e utilize um motor de correção automática para calcular notas de forma mais consistente e auditável.

## Funcionalidades Planejadas

### MVP

- Criação de avaliações
- Cadastro de questões
- Cadastro de gabaritos estruturados
- Inserção manual de respostas de alunos
- Correção automática por tipo de questão
- Cálculo da nota final
- Revisão manual de respostas
- Feedback por questão

### Tipos de questão iniciais

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização

## Tecnologias

### Backend

- .NET 8
- ASP.NET Core Web API
- C#
- Entity Framework Core

### Frontend

- Angular
- TypeScript
- Reactive Forms
- HttpClient

### Banco de Dados

- SQL Server ou PostgreSQL

### Testes

- xUnit
- FluentAssertions

## Arquitetura Planejada

O projeto será organizado seguindo uma separação simples de responsabilidades:

```txt
src/
  GradeFlow.Api/
  GradeFlow.Application/
  GradeFlow.Domain/
  GradeFlow.Infrastructure/
