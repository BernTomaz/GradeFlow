# Arquitetura

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

## Diretrizes

- Controllers devem ser finos.
- Regras de negócio devem ficar em services, domain ou no motor de correção.
- O motor de correção deve usar Strategy Pattern.
- IA, OCR, upload, relatórios e login complexo não devem ser prioridade antes do MVP.
