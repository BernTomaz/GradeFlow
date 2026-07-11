# GradeFlow

Sistema web para criação de avaliações, cadastro de gabaritos estruturados, correção automática de respostas e revisão manual de notas.

## Objetivo

O GradeFlow tem como objetivo apoiar professores, avaliadores e instituições no processo de correção de atividades, provas e trabalhos.

A proposta do projeto é permitir o cadastro de avaliações com questões estruturadas, configuração de gabaritos por tipo de questão e uso de um motor de correção automática para calcular notas de forma consistente, rastreável e auditável.

Além de servir como uma solução educacional, o GradeFlow é utilizado como projeto de portfólio para demonstrar conhecimentos em:

- Arquitetura em camadas
- ASP.NET Core Web API
- Angular
- Entity Framework Core
- SQL Server
- JWT Authentication
- Testes automatizados
- Boas práticas de engenharia de software

---

## Status do Projeto

> Projeto em desenvolvimento ativo.

O MVP principal já possui backend, frontend Angular, correção automática, revisão manual, auditoria, testes automatizados e autenticação baseada em perfis.

### ✅ Implementado

#### Domínio e Correção

- Modelagem de domínio:
  - Assignment
  - Question
  - AnswerKey
  - StudentAnswer
  - Submission
  - CorrectionResult
  - CorrectionLog
  - User
- Motor de correção automática utilizando Strategy Pattern
- Correção de múltipla escolha
- Correção de verdadeiro ou falso
- Correção numérica com tolerância configurável
- Correção de texto curto com normalização de acentos, pontuação e espaços
- Correção completa da submissão
- Recorreção de questão individual
- Registro detalhado de logs de correção
- Revisão manual de respostas específicas

#### Backend

- ASP.NET Core Web API
- Arquitetura em camadas inspirada em Clean Architecture
- Serviços de aplicação:
  - AssignmentService
  - AuthService
  - CorrectionService
  - QuestionService
  - SubmissionService
- Repository Pattern
- Entity Framework Core
- SQL Server
- JWT Authentication
- Refresh Token
- Controle de acesso baseado em perfis:
  - Admin
  - Teacher
  - Student
- Autorização por proprietário da avaliação
- Swagger/OpenAPI
- CORS configurado para integração com Angular
- Testes automatizados

#### Frontend

- Angular 20
- Standalone Components
- TypeScript
- Reactive Forms
- HttpClient
- Route Guards
- HTTP Interceptors
- Login e Cadastro
- CRUD de avaliações
- CRUD de questões
- CRUD de submissões
- Visualização de resultados de correção

### 🔜 Planejado

- Pipeline CI/CD com GitHub Actions
- Docker e Docker Compose
- Deploy público para demonstração
- Relatórios de desempenho por turma
- Exportação de notas
- Dashboard com métricas
- Cobertura ampliada de testes

---

## Tipos de Questão Suportados

- Múltipla escolha
- Verdadeiro ou falso
- Numérica com tolerância
- Texto curto com normalização

---

## Tecnologias Utilizadas

### Backend

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication

### Frontend

- Angular 20
- TypeScript
- Reactive Forms
- HttpClient

### Banco de Dados

- SQL Server

### Testes

- xUnit
- FluentAssertions

### Ferramentas

- Git
- GitHub
- Swagger/OpenAPI

---

## Arquitetura

O projeto segue uma separação em camadas inspirada em Clean Architecture.

```txt
src/
│
├── GradeFlow.Api/
│   ├── Controllers
│   ├── Middlewares
│   └── Program.cs
│
├── GradeFlow.Application/
│   ├── Services
│   ├── DTOs
│   ├── Interfaces
│   └── Strategies
│
├── GradeFlow.Domain/
│   ├── Entities
│   ├── Enums
│   └── Contracts
│
├── GradeFlow.Infrastructure/
│   ├── Data
│   ├── Repositories
│   └── Migrations
│
└── GradeFlow.Web/
    ├── Core
    ├── Features
    └── Shared

tests/
└── GradeFlow.Tests/
```

---

## Pré-requisitos

Antes de executar o projeto, instale:

- .NET 10 SDK
- Node.js 22 ou superior
- Angular CLI
- SQL Server LocalDB, Express ou Developer
- Git
- EF Core Tools

Instalação da ferramenta do Entity Framework Core:

```powershell
dotnet tool install --global dotnet-ef
```

Verificar versões instaladas:

```powershell
dotnet --version
node --version
npm --version
dotnet ef --version
```

---

## Instalação

Clone o repositório:

```bash
git clone https://github.com/BernTomaz/GradeFlow.git
cd GradeFlow
```

Restaurar dependências do backend:

```powershell
dotnet restore
```

Instalar dependências do frontend:

```powershell
cd src/GradeFlow.Web
npm install
```

---

## Configuração do Banco de Dados

O projeto utiliza SQL Server com Entity Framework Core Migrations.

### Configurar Connection String

Edite o arquivo:

```txt
src/GradeFlow.Api/appsettings.json
```

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GradeFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> ⚠️ Este `appsettings.json` é destinado ao ambiente de desenvolvimento local. Em ambientes produtivos, utilize variáveis de ambiente, User Secrets ou serviços de gerenciamento de segredos.

### Aplicar Migrations

Como a solução possui múltiplos projetos, execute o comando a partir da raiz do repositório:

```powershell
dotnet ef database update --project src\GradeFlow.Infrastructure --startup-project src\GradeFlow.Api
```

As migrations versionadas no projeto irão criar automaticamente:

- Banco de dados
- Tabelas
- Relacionamentos
- Índices
- Estrutura inicial do sistema

### Migrations Existentes

- InitialCreate
- RequireStudentAnswerAnswer
- AddCorrectionLogs
- AddUsers
- AddUserOwnership

---

## Executando o Backend

No Visual Studio, selecione o perfil:

```txt
https
```

Ou execute via terminal:

```powershell
dotnet run --project src\GradeFlow.Api --launch-profile https
```

A API estará disponível em:

```txt
https://localhost:7013
```

Documentação Swagger:

```txt
https://localhost:7013/swagger
```

---

## Executando o Frontend

Abra um novo terminal:

```powershell
cd src\GradeFlow.Web
npm start
```

Aplicação disponível em:

```txt
http://localhost:4200
```

O frontend utiliza o arquivo `proxy.conf.json` para encaminhar automaticamente chamadas `/api` para a API local.

---

## Endpoints Principais

| Recurso | Endpoints |
|----------|----------|
| Assignments | GET, POST `/api/assignments` |
| Assignment por Id | GET, PUT, DELETE `/api/assignments/{id}` |
| Questions | GET, POST `/api/assignments/{assignmentId}/questions` |
| Question por Id | GET, PUT, DELETE `/api/questions/{id}` |
| Submissions | GET, POST `/api/assignments/{assignmentId}/submissions` |
| Submission por Id | GET, PUT, DELETE `/api/submissions/{id}` |
| Correção | POST `/api/submissions/{id}/correct` |
| Correção de Questão | POST `/api/submissions/{id}/questions/{questionId}/correct` |
| Revisão Manual | PUT `/api/student-answers/{answerId}/review` |
| Auditoria | GET `/api/submissions/{id}/correction-logs` |
| Auth | POST `/api/auth/register` |
| Login | POST `/api/auth/login` |
| Refresh Token | POST `/api/auth/refresh-token` |

---

## Executando os Testes

```powershell
dotnet test
```

Os testes cobrem:

- Serviços de aplicação
- Regras de negócio
- Estratégias de correção
- Fluxos de autenticação

---

## Roadmap

### Curto Prazo

- CI/CD com GitHub Actions
- Docker Compose
- Deploy automatizado

### Médio Prazo

- Relatórios por turma
- Exportação de notas
- Dashboard administrativo

### Longo Prazo

- Integração com plataformas educacionais
- Correção assistida por IA
- Análise estatística de desempenho

---

## Licença

Este projeto é disponibilizado para fins educacionais, estudo e demonstração de portfólio profissional.
