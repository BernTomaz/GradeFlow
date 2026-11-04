GradeFlow - Fluxo 06 - Relatórios e Exportações

Objetivo
Transformar os resultados das correções em informações úteis para professor e instituição.

usuário principal
Professor.

Fluxo funcional
1. Professor abre uma avaliação.
2. Acessa a tela de relatório.
3. Sistema busca submissões corrigidas.
4. Sistema calcula indicadores.
5. Sistema exibe resumo da turma.
6. Professor pode exportar os resultados.

Relatórios iniciais
- Nota por aluno
- Média da turma
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

Exportação Excel:
- ClosedXML

Exportação PDF:
- QuestPDF

Frontend:
- Angular Component de relatório
- Tabela de resultados
- Cards simples de indicadores
- Botao para exportar

Regra importante
Relatório só deve vir depois da correção funcionar.
Dashboard bonito antes do motor de correção e desperdício.
Adicionar bibliotecas de exportação apenas quando a exportação for implementada.

Proteção da exportação CSV
- Nome e email que começam com indicadores de fórmula (`=`, `+`, `-`, `@` e variantes de largura completa), mesmo após espaços ou quebras de linha, são exportados com tabulação inicial dentro de um campo entre aspas.
- Aspas e separadores continuam escapados. Os dados armazenados não são alterados.
- A tabulação faz parte do campo exportado e pode ser percebida por consumidores programáticos. Esta proteção é destinada à abertura em planilhas; o comportamento após regravar o arquivo depende do aplicativo utilizado.
