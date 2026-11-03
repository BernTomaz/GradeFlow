# Validação local — 03/11/2026

Ambiente: Docker Compose local, API em `http://localhost:8080` e frontend em `http://localhost:4200`.
Foram usados registros fictícios identificados por `QA 2026-11-03`, preservando a avaliação de demonstração existente.

## Resultados pela interface

| Fluxo | Resultado e evidência |
| --- | --- |
| Login Admin | Entrada com a conta local de demonstração e carregamento do dashboard confirmados. |
| Logout e rota protegida | Após sair, abrir o detalhe de uma avaliação redirecionou ao login. |
| Avaliação | Criação, edição e atualização da página confirmadas. Avaliação descartável excluída após autorização; ausência confirmada após recarregar. |
| Questões | Criados os quatro tipos. Editadas questões de múltipla escolha e numérica. |
| Cadastro de usuários | Criadas, com autorização, as contas fictícias Professor e Aluno. O primeiro login de ambas exigiu troca de senha. |
| Troca obrigatória | O Professor continuou na tela de troca ao tentar acessar o dashboard. A troca em si não foi concluída, por escolha do usuário. |
| Alteração de nome | Nome da conta de demonstração alterado e depois restaurado para `Usuario Teste`. |
| Submissão manual | Cadastradas e persistidas respostas dos quatro tipos. Nome do aluno editado, persistido e restaurado, preservando nota e respostas. |
| CSV válido | Importados nome com vírgula e resposta com aspas e quebra de linha; conteúdo conferido na submissão. |
| CSV inválido | Registro com coluna extra rejeitado com mensagem da linha; nenhuma submissão desse arquivo apareceu na lista. |
| Correção automática | Respostas corretas receberam 4 de 4. `10,05` aceito para gabarito `10` com tolerância `0,1`; `FOTOSSINTESE!` aceito para `fotossíntese`. Texto diferente recebeu zero. |
| Recorreção individual | Resposta numérica editada para `10.2` recebeu zero; editada para `10.05`, voltou a receber um ponto. Total atualizado. |
| Revisão manual e auditoria | Nota de uma resposta alterada para `0,5`, total recalculado para `3,5` e registro de auditoria persistido após recarregar a página. |
| Relatório | Antes da limpeza, dois alunos com notas `3,5` e `3`: média `3,25`, maior `3,5` e menor `3`. |
| Exportações | Downloads CSV, Excel e PDF iniciados pela interface e concluídos. CSV conferido pelos nomes e notas; Excel conferido pelas duas planilhas e nomes; PDF conferido pela assinatura e fechamento do arquivo. Layout interno de Excel/PDF não foi revisado visualmente. |
| Exclusão de submissão | Submissão fictícia `QA Silva, Ana` excluída com autorização e removida da lista operacional. |
| Navegação e tela pequena | Recarregamento de rotas internas funcionou. Relatório verificado em largura de 390 px, sem transbordamento horizontal; tamanho normal restaurado. |

## Falha encontrada e corrigida

O campo HTML numérico fornecia um número ao formulário, mas `answerKey.correctAnswer` deve ser texto no contrato da API. Isso impedia salvar questões numéricas.

O envio agora converte o gabarito para texto, no mesmo ponto usado pela criação e edição. Dois testes de formulário reproduziram a falha antes da alteração e passaram depois. Criação e edição foram repetidas com sucesso no frontend Docker reconstruído.

- Suíte frontend após a correção: **12 testes aprovados**.
- Build de produção do frontend Docker: concluído; aviso de pacote inicial de aproximadamente 641 kB para meta de 600 kB.
- API, frontend e SQL Server: saudáveis ao final.
- Backend não foi alterado; sua suíte tinha 67 testes aprovados na validação anterior e não foi repetida nesta rodada.

## Pendências e limites

- Usuário optou por não concluir a troca obrigatória das contas fictícias. Permissões de Professor e Aluno após essa troca não foram verificadas pela interface.
- Alteração e redefinição de senha não foram executadas. Apenas o formulário e o redirecionamento obrigatório foram observados.
- Exclusão isolada de questão não foi exercitada.
- Responsividade foi amostrada no relatório; não representa revisão de todas as telas e tamanhos.
- Alertas de segurança da dependência `System.Security.Cryptography.Pkcs` 9.0.4, observados no build anterior, continuam pendentes de avaliação antes do deploy público.

## Registros mantidos

- Avaliação: `QA 2026-11-03 — Fluxos GradeFlow` (`aaf288a7-0b97-407d-bf61-870e07bf97ac`).
- Submissão: `QA Aluna Manual`, com nota revisada `3,5`.
- Contas locais fictícias: `qa.professor.20261103@example.test` e `qa.aluno.20261103@example.test`, ainda com troca obrigatória pendente.
- Avaliação descartável removida definitivamente; submissão CSV removida por exclusão lógica.

Esta rodada confirma os cenários descritos, sem substituir uma validação completa de segurança ou uma aprovação de deploy público.
