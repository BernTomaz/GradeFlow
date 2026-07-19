GradeFlow - Visao Geral e Tecnologias

Objetivo do sistema
Criar uma plataforma web para professores criarem avaliações, cadastrarem questões e gabaritos estruturados, receberem respostas de alunos, corrigirem automaticamente quando possível e revisarem manualmente quando necessário.

Ideia central
O sistema não deve ser apenas um comparador de texto.
O ponto forte deve ser o motor de correção, onde cada tipo de questão tem uma regra própria.

Fluxo macro
1. Professor cria uma avaliação.
2. Professor cadastra questões.
3. Professor cadastra gabaritos.
4. Aluno ou professor insere respostas.
5. Sistema executa a correção automática.
6. Sistema calcula a nota final.
7. Professor revisa manualmente respostas duvidosas.
8. Sistema gera resultado e relatórios.

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

autenticação:
- JWT
- Refresh Token em etapa posterior

Arquitetura:
- Clean Architecture simples
- separação em Api, Application, Domain e Infrastructure

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
- Configuracoes da aplicação

GradeFlow.Application:
- DTOs
- Services
- Validacoes
- Casos de uso

GradeFlow.Domain:
- Entidades
- Enums
- Regras puras de negocio
- Interfaces do motor de correção

GradeFlow.Infrastructure:
- Entity Framework Core
- DbContext
- Migrations
- Repositorios
- Integracoes externas

Prioridade real do MVP
Não comece por IA, OCR, dashboard ou login complexo.
Comece pelo fluxo manual completo e pelo motor de correção.
