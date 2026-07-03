# GradeFlow

Sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

## Objetivo

O GradeFlow tem como objetivo apoiar professores, avaliadores e instituições no processo de correção de atividades, provas e trabalhos.

A proposta do projeto é permitir o cadastro de avaliações com questões estruturadas, configuração de gabaritos por tipo de questão e uso de um motor de correção automática para calcular notas de forma mais consistente, rastreável e auditável.

## Funcionalidades Planejadas

### MVP

- Criação de avaliações
- Cadastro de questões
- Cadastro de gabaritos estruturados
- Inserção manual de respostas de alunos
- Correção automática por tipo de questão
- Cálculo da nota final
- Feedback por questão
- Revisão manual de respostas

### Tipos de questão iniciais

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização

## Tecnologias

### Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core

### Frontend

- Angular
- TypeScript
- Reactive Forms
- HttpClient

### Banco de Dados

- SQL Server

### Testes

- xUnit
- FluentAssertions

## Arquitetura Planejada

O projeto é organizado seguindo uma separação simples de responsabilidades. A implementação segue a ordem definida em `docs/etapas/`, mantendo a sequência oficial do projeto.

```txt
src/
  GradeFlow.Api/
  GradeFlow.Application/
  GradeFlow.Domain/
  GradeFlow.Infrastructure/
  GradeFlow.Web/
```

## Como Rodar Localmente

### Backend

No Visual Studio, selecione o perfil `https` do projeto `GradeFlow.Api`.

Ou rode pelo terminal:

```powershell
dotnet run --project src\GradeFlow.Api --launch-profile https
```

A API deve ficar disponível em:

```txt
https://localhost:7013
```

### Frontend

Em outro terminal:

```powershell
cd src\GradeFlow.Web
npm start
```

Abra:

```txt
http://localhost:4200
```

O frontend usa `proxy.conf.json` para encaminhar chamadas `/api` para `https://localhost:7013`.
