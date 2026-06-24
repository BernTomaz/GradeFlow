GradeFlow - Visao Geral e Tecnologias

Objetivo do sistema
Criar uma plataforma web para professores criarem avaliacoes, cadastrarem questoes e gabaritos estruturados, receberem respostas de alunos, corrigirem automaticamente quando possivel e revisarem manualmente quando necessario.

Ideia central
O sistema nao deve ser apenas um comparador de texto.
O ponto forte deve ser o motor de correcao, onde cada tipo de questao tem uma regra propria.

Fluxo macro
1. Professor cria uma avaliacao.
2. Professor cadastra questoes.
3. Professor cadastra gabaritos.
4. Aluno ou professor insere respostas.
5. Sistema executa a correcao automatica.
6. Sistema calcula a nota final.
7. Professor revisa manualmente respostas duvidosas.
8. Sistema gera resultado e relatorios.

Tecnologias recomendadas
Backend:
- .NET 10 Web API
- C#
- ASP.NET Core

Frontend:
- Angular
- TypeScript
- Angular Reactive Forms

Banco de dados:
- SQL Server

ORM:
- Entity Framework Core

Autenticacao:
- JWT
- Refresh Token em etapa posterior

Arquitetura:
- Clean Architecture simples
- Separacao em Api, Application, Domain e Infrastructure

Projetos sugeridos
src/
  GradeFlow.Api/
  GradeFlow.Application/
  GradeFlow.Domain/
  GradeFlow.Infrastructure/

Responsabilidades
GradeFlow.Api:
- Controllers
- Swagger
- Middlewares
- Configuracoes da aplicacao

GradeFlow.Application:
- DTOs
- Services
- Validacoes
- Casos de uso

GradeFlow.Domain:
- Entidades
- Enums
- Regras puras de negocio
- Interfaces do motor de correcao

GradeFlow.Infrastructure:
- Entity Framework Core
- DbContext
- Migrations
- Repositorios
- Integracoes externas

Prioridade real do MVP
Nao comece por IA, OCR, dashboard ou login complexo.
Comece pelo fluxo manual completo e pelo motor de correcao.
