# Testes

## Backend

```powershell
dotnet clean GradeFlow.slnx -m:1
dotnet restore GradeFlow.slnx
dotnet build GradeFlow.slnx --no-restore -m:1
dotnet test GradeFlow.slnx --no-build -m:1
```

## Frontend

```powershell
cd src\GradeFlow.Web
npm ci
npm run build
npm test -- --watch=false --browsers=ChromeHeadless
```

Nesta validação, `npm ci` é intencional para reproduzir o comportamento esperado no pipeline.

No ambiente local em Windows/OneDrive, `dotnet clean GradeFlow.slnx` pode falhar ao remover arquivos temporários quando o MSBuild executa projetos em paralelo. Use `-m:1` localmente; no CI, prefira workspace limpo e não dependa de `clean` sem necessidade.

Os testes cobrem:

- Serviços de aplicação
- Regras de negócio
- Estratégias de correção
- Fluxos de autenticação
- Bloqueio de troca obrigatória pela API, revogação após mudança ou redefinição de senha e rejeição de sessões de usuários removidos ou com perfil alterado
- Substituição integral da sessão no frontend após alteração de senha
- Proteção contra fórmulas em campos textuais exportados para CSV
- Importação CSV com quatro separadores, campos entre aspas, aspas escapadas, quebras de linha e rejeição de estrutura inválida
- Mensagens de erro no frontend, incluindo validação por campo e falhas de conexão
- Tratamento de datas da API com e sem fuso horário explícito
- Envio do gabarito numérico como texto na criação e edição de questões

Validação pela interface no Docker: [resultados de 03/11/2026](operacao/validacao-local-2026-11-03.md).

Validação de segurança e dependências: [ajustes de 04/11/2026](operacao/ajustes-seguranca-2026-11-04.md).
