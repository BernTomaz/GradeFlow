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

Validação de perfis e tratamento de acesso negado: [resultados de 05/11/2026](operacao/validacao-permissoes-2026-11-05.md), com 90 testes backend e 20 frontend aprovados.

## Verificação de 06/11/2026

Resultado consolidado, conforme saídas do terminal fornecidas pelo usuário: **92 testes backend aprovados, zero falhas e zero ignorados**, em 15,3 segundos, com `dotnet test GradeFlow.slnx -m:1`; **21 testes frontend aprovados** em ChromeHeadless 156 (`TOTAL: 21 SUCCESS`). O Docker concluiu os builds da API e do frontend; SQL Server e API ficaram saudáveis, frontend iniciado e inicialização do banco encerrada. A validação automatizada está concluída. O fluxo completo pela interface permanece pendente.

Foram acrescentadas regressões para leitura de questões pelo Aluno sem gabarito, bloqueio de alterações administrativas, submissão própria, ocultação do gabarito nos logs, identificação do revisor e preservação do histórico de recorreções. No frontend, há uma regressão do detalhe de avaliação para Aluno, incluindo retorno das ações ao trocar para Professor.

Os builds backend e frontend foram concluídos. O bundle inicial ficou em 647,34 kB, acima da meta de aviso de 600 kB. As falhas de execução descritas abaixo ocorreram anteriormente na sessão restrita do agente; foram superadas pela execução das duas suítes no terminal do usuário.

### Histórico do bloqueio na sessão restrita

Na nova rodada, nove verificações direcionadas passaram por invocação direta dos métodos de teste existentes, com as asserções FluentAssertions preservadas: leitura de questões sem gabarito, bloqueio de alterações pelo Aluno, submissão própria e ocultação de gabarito nos logs, identificação da revisão manual, histórico de recorreção, correção individual e quatro combinações de perfil/vínculo da avaliação. O executor temporário em `TestResults/verification-runner` foi compilado com `-m:1 -p:UseAppHost=false` e executado com `dotnet TestResults/verification-runner/bin/Debug/net10.0/Verify.dll`, encerrando com código zero. Essa verificação parcial não equivale à execução da suíte xUnit completa. A restauração do executor emitiu NU1900 por indisponibilidade do serviço de auditoria NuGet.

Naquela rodada restrita, `dotnet test` foi anulado após o limite de 30 segundos, ChromeHeadless falhou nas três inicializações e o Docker negou acesso. Esses resultados anteriores não substituem os resultados aprovados das suítes completas registrados acima.

- O log do processo `testhost` confirmou `System.Net.Sockets.SocketException (10013)` ao conectar ao executor VSTest em `127.0.0.1`: acesso ao soquete proibido pelas permissões. O problema aparece também em `--list-tests`, antes de executar os testes; portanto, o travamento observado não identifica um teste defeituoso. Forçar o host `dotnet` em vez de `testhost.exe` manteve a mesma falha.
- A tentativa alternativa de Karma com o executável do Microsoft Edge também falhou nas três inicializações; nenhum resultado Jasmine foi produzido. A causa específica da falha do navegador não foi confirmada.
- `docker compose ps` retornou acesso negado ao pipe `dockerDesktopLinuxEngine`. A aplicação não estava acessível na porta local 4200, e o navegador integrado falhou ao abrir a sessão. O fluxo completo do Aluno pela interface não foi validado nesta rodada.
- Pendência atual: conferir o fluxo completo pela interface com uma conta de Aluno. Nenhuma proteção do sistema foi desativada pelo agente para executar os testes.
