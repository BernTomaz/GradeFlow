# GradeFlow.Web

Frontend Angular do projeto GradeFlow.

Esta aplicação é responsável pela interface de usuário do sistema, consumindo os endpoints disponibilizados pela API `GradeFlow.Api`.

> Para configuração completa do backend e banco de dados, veja o [README principal](../../README.md).

## Tecnologias

- Angular 20
- TypeScript
- Reactive Forms
- HttpClient
- Angular Router
- Route Guards
- HTTP Interceptors

---

## Pré-requisitos

Antes de executar o frontend, certifique-se de possuir:

- Node.js 22 ou superior
- npm
- Angular CLI

Verificar versões:

```powershell
node --version
npm --version
ng version
```

> Se o Angular CLI não estiver instalado globalmente, use `npx` antes dos comandos `ng` deste README (ex: `npx ng serve`, `npx ng test`).

---

## Configuração

O frontend depende da API `GradeFlow.Api`.

Inicie primeiro o backend:

```txt
https://localhost:7013
```

A URL da API pode ser configurada através dos arquivos de ambiente:

```txt
src/environments/
├── environment.ts
└── environment.development.ts
```

Exemplo ilustrativo (a estrutura pode variar conforme a versão atual do projeto):

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7013/api'
};
```

> Ajuste `apiUrl` (ou o nome da propriedade equivalente usada no projeto) caso a API rode em outra porta ou host.

---

## Instalação

Instalar dependências:

```powershell
npm install
```

---

## Executando Localmente

Iniciar o servidor de desenvolvimento:

```powershell
npm start
```

Ou:

```powershell
ng serve
```

A aplicação ficará disponível em:

```txt
http://localhost:4200
```

O projeto utiliza `proxy.conf.json` para encaminhar chamadas `/api` para a API local, evitando problemas de CORS durante o desenvolvimento.

---

## Build

Gerar build de produção:

```powershell
npm run build
```

Os arquivos serão gerados em:

```txt
dist/
```

---

## Testes

Executar testes unitários:

```powershell
ng test
```

---

## Estrutura do Projeto

```txt
src/
├── app/
│   ├── core/
│   │   ├── services/
│   │   ├── guards/
│   │   └── interceptors/
│   │
│   ├── features/
│   │   ├── assignments/
│   │   ├── questions/
│   │   ├── submissions/
│   │   ├── correction/
│   │   └── auth/
│   │
│   └── shared/
│
├── environments/
└── assets/
```

---

## Funcionalidades

- Autenticação com JWT
- Cadastro e login de usuários
- Gerenciamento de avaliações
- Gerenciamento de questões
- Gerenciamento de submissões
- Visualização de correções
- Controle de acesso por perfil
- Integração com API REST do GradeFlow

> Algumas funcionalidades exigem autenticação e permissões específicas de usuário (Admin, Teacher ou Student).
