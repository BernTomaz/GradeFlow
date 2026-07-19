GradeFlow - Etapa 11 - importação, relatórios e exportação

Objetivo
Aumentar a produtividade do professor depois que o MVP estiver funcionando.

Status
Implementada no projeto atual.

Tecnologias sugeridas
CSV:
- Leitura simples nativa, se o formato continuar básico
- CsvHelper, se houver muitos casos especiais de CSV

Excel:
- ClosedXML

PDF:
- QuestPDF

Backend:
- ASP.NET Core
- EF Core
- LINQ

Frontend:
- Angular
- Upload com FormData
- Tabelas de relatório

importação CSV
Endpoint:
POST /api/assignments/{assignmentId}/submissions/import

Formato aceito:
CSV com colunas `student_name`, `student_email`, `q1`, `q2`, `q3`, `q4`.

Exemplo:
student_name,student_email,q1,q2,q3,q4
Ana,ana@email.com,A,10,Resposta curta,V

Validacoes:
- Arquivo vazio
- Colunas obrigatórias ausentes
- questão inexistente
- Resposta invalida
- Linha duplicada

relatórios
Endpoint:
GET /api/assignments/{assignmentId}/report

Indicadores:
- Nota por aluno
- Media da turma
- Maior nota
- Menor nota
- questões com mais erros
- questões com mais acertos

exportação
Endpoints:
GET /api/assignments/{assignmentId}/export/csv
GET /api/assignments/{assignmentId}/export/excel
GET /api/assignments/{assignmentId}/export/pdf

Observacao:
- A importação atual aceita CSV.
- Excel e PDF estão disponíveis apenas para exportação.

Ponto de atenção
relatório e importação só fazem sentido depois que a correção base estiver confiável.
Não adicionar biblioteca antes de confirmar que a funcionalidade precisa dela.
