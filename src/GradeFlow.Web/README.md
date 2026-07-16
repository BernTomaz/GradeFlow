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

- Node.js 20.19 ou superior
- npm
- Angular CLI

Verificar versões:

```powershell
node --version
npm --version
ng version
```

> Os comandos principais usam os scripts do `package.json`, então não é necessário instalar o Angular CLI globalmente.

---

## Configuração

O frontend depende da API `GradeFlow.Api`.

Inicie primeiro o backend:

```txt
https://localhost:7013
```

Em desenvolvimento, as chamadas usam caminhos relativos `/api` e são encaminhadas pelo `proxy.conf.json` para `https://localhost:7013`.

---

## Instalação

Instalar dependências:

```powershell
npm install
```

Para instalação limpa, validação antes de commit ou ambiente de CI:

```powershell
npm ci
```

---

## Executando Localmente

Iniciar o servidor de desenvolvimento:

```powershell
npm start
```

Ou:

```powershell
npx ng serve --proxy-config proxy.conf.json
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
dist/GradeFlow.Web/browser/
```

---

## Testes

Executar testes unitários:

```powershell
npm test -- --watch=false --browsers=ChromeHeadless
```

---

## Estrutura do Projeto

```txt
src/
├── app/
│   ├── core/
│   │   ├── api/
│   │   ├── auth/
│   │   └── models/
│   │
│   ├── features/
│   │   ├── assignments/
│   │   ├── questions/
│   │   ├── submissions/
│   │   ├── correction/
│   │   └── auth/
│   │
│   └── shared/
```

---

## Funcionalidades

- Autenticação com JWT
- Cadastro e login de usuários
- Alteração de senha autenticada
- Gerenciamento de avaliações
- Gerenciamento de questões
- Gerenciamento de submissões
- Visualização de correções
- Controle de acesso por perfil
- Integração com API REST do GradeFlow

> Algumas funcionalidades exigem autenticação e permissões específicas de usuário (Admin, Teacher ou Student).
