GradeFlow - Etapa 11 - Importacao, Relatorios e Exportacao

Objetivo
Aumentar a produtividade do professor depois que o MVP estiver funcionando.

Tecnologias
CSV:
- CsvHelper

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
- Tabelas de relatorio

Importacao
Endpoint:
POST /api/assignments/{assignmentId}/submissions/import

Formato sugerido:
student_name | student_email | q1 | q2 | q3 | q4

Validacoes:
- Arquivo vazio
- Colunas obrigatorias ausentes
- Questao inexistente
- Resposta invalida
- Linha duplicada

Relatorios
Endpoint:
GET /api/assignments/{assignmentId}/report

Indicadores:
- Nota por aluno
- Media da turma
- Maior nota
- Menor nota
- Questoes com mais erros
- Questoes com mais acertos

Exportacao
Endpoints:
GET /api/assignments/{assignmentId}/export/excel
GET /api/assignments/{assignmentId}/export/pdf

Ponto de atencao
Relatorio e importacao so fazem sentido depois que a correcao base estiver confiavel.
