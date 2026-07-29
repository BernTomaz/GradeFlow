# Arquitetura

O projeto segue uma separação em camadas inspirada em Clean Architecture.

```txt
src/
│
├── GradeFlow.Api/
│   ├── Controllers
│   ├── Services
│   └── Program.cs
│
├── GradeFlow.Application/
│   ├── Services
│   ├── DTOs
│   ├── Repositories
│   └── Corrections
│
├── GradeFlow.Domain/
│   ├── Entities
│   ├── Enums
│   └── Corrections
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
