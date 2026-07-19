GradeFlow - Fluxo 08 - importação CSV

Objetivo
Permitir importar respostas de muitos alunos por CSV sem cadastrar tudo manualmente.

Status
Implementado para CSV.
importação XLSX continua como recurso futuro.

usuário principal
Professor.

Fluxo funcional
1. Professor abre uma avaliação.
2. Clica em importar respostas via CSV.
3. Seleciona arquivo CSV.
4. Sistema valida o arquivo.
5. Sistema identifica colunas de aluno e respostas.
6. Sistema cria submissões.
7. Sistema cria StudentAnswers.
8. Sistema executa correção em lote.
9. Sistema exibe resumo da importação.

Formato sugerido
student_name | student_email | q1 | q2 | q3 | q4
Joao         | joao@email.com | B  | 10 | fotossintese | C
Maria        | maria@email.com| A  | 9.8| fotossintese | C

Endpoint recomendado
POST /api/assignments/{assignmentId}/submissions/import

Formato atual:
- CSV

Excel:
- importação XLSX ainda Não implementada.
- ClosedXML deve ser usado apenas se a importação XLSX for adicionada.

DTO de retorno sugerido
ImportSubmissionResponse:
- TotalRows
- ImportedRows
- FailedRows
- Errors
- CreatedSubmissions

Tecnologias envolvidas
Backend:
- ASP.NET Core Upload
- IFormFile
- Application Service
- validação de arquivo

Leitura de CSV:
- Parser simples no service de submissão
- CsvHelper deve ser considerado apenas se o formato passar a exigir separadores escapados, aspas complexas ou mais variacoes

Leitura de Excel:
- Não implementada no fluxo atual
- ClosedXML deve ser usado se a importação XLSX for adicionada no futuro

Banco:
- EF Core
- Insercao em lote
- Transacao

Frontend:
- Angular upload component
- FormData
- HttpClient
- Tela de resumo de importação

Validacoes
- Arquivo vazio
- Extensao invalida
- MIME type invalido
- Colunas obrigatórias ausentes
- questão inexistente
- Resposta numérica invalida
- Linha duplicada
- Erros parciais por linha

Regra importante
O sistema aceita importação de submissões por CSV.
Excel e PDF estão disponíveis apenas para exportação.
