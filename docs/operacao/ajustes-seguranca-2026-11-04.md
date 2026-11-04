# Ajustes de segurança — 04/11/2026

## Alterações

- A API consulta o usuário atual ao validar uma sessão e exige troca de senha também fora da interface. Uma conta com troca pendente recebe `403` nos endpoints protegidos, exceto na alteração de senha.
- Tokens incluem uma marca derivada do hash armazenado da senha, sem expor `PasswordHash`. Alterar ou redefinir a senha invalida as sessões anteriores; usuário removido ou com perfil alterado também perde o acesso anterior.
- A reemissão usa o usuário validado no mesmo pedido. Isso evita emitir uma sessão com credenciais novas a partir de um token antigo que estava sendo processado durante uma redefinição.
- A alteração de senha retorna `200` com a nova `AuthResponse`; o Angular substitui a sessão completa. Tokens emitidos antes desta atualização não são aceitos e exigem novo login.
- Campos textuais exportados no CSV são neutralizados quando começam com indicadores de fórmula, inclusive após espaços ou quebras de linha. A proteção usa tabulação inicial dentro do campo entre aspas. Aspas e separadores continuam escapados e os dados armazenados são preservados.
- Angular foi atualizado de `20.3.25` para `20.3.35`, com CLI e build `20.3.42`. O lockfile foi regenerado pelo npm, mantendo as versões principais existentes.
- A dependência transitiva `System.Security.Cryptography.Pkcs` foi fixada na versão corrigida `9.0.21` na infraestrutura.

Não houve migration, alteração do esquema do banco nem troca de senha de contas reais durante a validação.

## Validação

- **86 testes backend aprovados**, incluindo bloqueio de leitura e escrita antes da troca, novo token após alteração, revogação de tokens antigos na alteração e na redefinição, bloqueio de renovação, remoção do usuário, alteração de perfil e fórmulas com aspas e quebras de linha no CSV.
- **13 testes frontend aprovados**, incluindo substituição integral da sessão após alteração de senha.
- `dotnet list GradeFlow.slnx package --vulnerable --include-transitive`: nenhum pacote vulnerável apontado pelas fontes consultadas.
- `npm audit --omit=dev`: nenhum alerta nas dependências de produção.
- API e frontend reconstruídos com Docker Compose. Antes da atualização, a conta fictícia com troca pendente conseguia consultar avaliações (`200`). Após a atualização: token anterior `401`; token novo com troca pendente `403`; renovação com troca pendente `403`; Admin com sessão nova `200`.
- Login e dashboard conferidos no navegador, com os indicadores preservados e sem erros de console.
- Revisão independente do fluxo de autenticação não identificou bloqueadores; os casos adicionais indicados foram incluídos nos testes.

## Alertas residuais e limites

O audit completo do npm ainda aponta **22 alertas moderados**, sem alertas altos ou críticos. Todos ficam em ferramentas de desenvolvimento e suas dependências, incluindo as cadeias de `glob` e `minimatch` usadas por Angular CLI, build e Karma. As correções automáticas propostas incluem downgrades incompatíveis ou pacotes sem correção compatível. Não foram aplicados `--force`, substituições de API incompatíveis nem supressões de alertas.

Essas ferramentas devem processar somente código e padrões de arquivos de fontes confiáveis. Elas não compõem a imagem final Nginx do frontend, mas os alertas continuam relevantes para o ambiente de desenvolvimento e build. O resultado de produção sem alertas não representa ausência universal de vulnerabilidades.

Não foram repetidos todos os fluxos manuais de 03/11 nem testes de carga. A alteração e a redefinição de senha foram testadas em usuários isolados dos testes de integração; contas reais e as contas fictícias do banco local não tiveram suas senhas alteradas.

A proteção do CSV segue o comportamento documentado para planilhas, sem executar fórmulas em Excel ou LibreOffice. O prefixo de tabulação integra o conteúdo exportado; não existe sanitização universal para todos os aplicativos e fluxos de regravação.

## Referências

- [Eventos de validação JWT do ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerevents?view=aspnetcore-10.0)
- [Correção de segurança do Angular](https://github.com/angular/angular/security/advisories/GHSA-hh8m-fm6v-7cvg)
- [Aviso de segurança .NET para PKCS](https://github.com/advisories/GHSA-846f-f764-76mx)
- [OWASP — CSV Injection](https://owasp.org/www-community/attacks/CSV_Injection)
