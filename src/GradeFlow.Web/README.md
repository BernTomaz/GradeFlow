# GradeFlow.Web

Frontend Angular do GradeFlow.

## Como Rodar

Primeiro, deixe a API `GradeFlow.Api` rodando em:

```txt
https://localhost:7013
```

Depois, neste diretório:

```powershell
npm install
npm start
```

Abra:

```txt
http://localhost:4200
```

O `npm start` usa `proxy.conf.json` para encaminhar chamadas `/api` para a API.

## Comandos

```powershell
npm run build
ng test
```
