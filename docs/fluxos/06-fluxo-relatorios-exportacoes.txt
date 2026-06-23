GradeFlow - Fluxo 06 - Relatorios e Exportacoes

Objetivo
Transformar os resultados das correcoes em informacoes uteis para professor e instituicao.

Usuario principal
Professor.

Fluxo funcional
1. Professor abre uma avaliacao.
2. Acessa a tela de relatorio.
3. Sistema busca submisssoes corrigidas.
4. Sistema calcula indicadores.
5. Sistema exibe resumo da turma.
6. Professor pode exportar os resultados.

Relatorios iniciais
- Nota por aluno
- Media da turma
- Maior nota
- Menor nota
- Quantidade de submisssoes corrigidas
- Quantidade de submisssoes pendentes
- Questao com maior erro
- Questao com maior acerto

Endpoints recomendados
GET /api/assignments/{assignmentId}/report
GET /api/assignments/{assignmentId}/export/excel
GET /api/assignments/{assignmentId}/export/pdf

DTO sugerido
AssignmentReportResponse:
- AssignmentId
- AssignmentTitle
- TotalSubmissions
- CorrectedSubmissions
- PendingSubmissions
- AverageScore
- HighestScore
- LowestScore
- StudentResults
- QuestionStats

Tecnologias envolvidas
Backend:
- ASP.NET Core
- Application Service
- LINQ
- EF Core projections

Banco:
- Consultas otimizadas
- Evitar N+1 queries
- Buscar resultados em lote

Exportacao Excel:
- ClosedXML
- EPPlus, observando licenca

Exportacao PDF:
- QuestPDF
- Rotativa
- iText, observando licenca

Frontend:
- Angular Component de relatorio
- Tabela de resultados
- Cards simples de indicadores
- Botao para exportar

Regra importante
Relatorio so deve vir depois da correcao funcionar.
Dashboard bonito antes do motor de correcao e desperdicio.

