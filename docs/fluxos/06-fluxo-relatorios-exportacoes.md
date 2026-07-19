GradeFlow - Fluxo 06 - relatórios e Exportacoes

Objetivo
Transformar os resultados das correcoes em informacoes uteis para professor e instituicao.

usuário principal
Professor.

Fluxo funcional
1. Professor abre uma avaliação.
2. Acessa a tela de relatório.
3. Sistema busca submissões corrigidas.
4. Sistema calcula indicadores.
5. Sistema exibe resumo da turma.
6. Professor pode exportar os resultados.

relatórios iniciais
- Nota por aluno
- Media da turma
- Maior nota
- Menor nota
- Quantidade de submissões corrigidas
- Quantidade de submissões pendentes
- questão com maior erro
- questão com maior acerto

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

exportação Excel:
- ClosedXML

exportação PDF:
- QuestPDF

Frontend:
- Angular Component de relatório
- Tabela de resultados
- Cards simples de indicadores
- Botao para exportar

Regra importante
relatório so deve vir depois da correção funcionar.
Dashboard bonito antes do motor de correção e desperdicio.
Adicionar bibliotecas de exportação apenas quando a exportação for implementada.
