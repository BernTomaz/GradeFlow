# Validação de permissões — 05/11/2026

## Correções

- Professor só acessa e gerencia avaliações vinculadas ao seu usuário. A regra foi aplicada em avaliações, questões, submissões e correção.
- Avaliações sem `TeacherUserId`, inclusive as criadas pelo Admin, ficam disponíveis ao Admin. Nenhum registro foi removido ou atribuído automaticamente a um professor; não houve migration.
- O Aluno não vê os atalhos de criação de avaliação no menu, na lista ou no dashboard. Ações de edição administrativa, correção e exclusão também ficam ocultas no detalhe de submissão para esse perfil. As permissões continuam sendo impostas pela API.
- Detalhes de avaliação, submissão e relatório tratam falhas de carregamento com mensagem e link de retorno. Respostas 403 e 404 recebem mensagens claras, sem revelar se um recurso de outro usuário existe.

## Verificação

- 90 testes backend aprovados. A regressão de acesso sem professor foi reproduzida antes da correção. Casos cobrem Admin, Professor proprietário, Professor sem vínculo e Aluno, incluindo bloqueio de exclusões e preservação do registro.
- 20 testes frontend aprovados. Reintroduzir temporariamente a permissão indevida fez os três testes de botões falharem; código restaurado e suíte final aprovada.
- Builds de produção de API e frontend concluídos no Docker; API, frontend e SQL Server saudáveis, volume preservado.
- No navegador: trocas obrigatórias das duas contas fictícias concluídas pelo usuário; Aluno sem atalho de nova avaliação; submissão alheia e relatório bloqueados com mensagem e retorno, sem erros de aplicação no console.
- Professor sem acesso à avaliação QA sem responsável. Admin manteve acesso ao dashboard e ao relatório com quatro questões, uma submissão e nota 3,5.
- Sessões usadas na conferência foram encerradas.

## Limites

- Ainda falta validar pela interface uma submissão pertencente ao próprio Aluno. Esta rodada não cria mecanismos novos de distribuição de avaliações ou vínculo de responsáveis.
- Avaliações anteriores sem responsável deixam de aparecer para Professores e continuam acessíveis ao Admin. Uma futura atribuição deve identificar explicitamente o responsável; não se deve restaurar acesso global como alternativa.
- Build mantém o aviso de tamanho inicial do frontend (aproximadamente 646 kB para meta de 600 kB). Os alertas de dependências descritos no relatório de 04/11 não foram reavaliados nesta rodada.
- Deploy público e testes de carga não foram realizados.
