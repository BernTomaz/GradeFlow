GradeFlow - Fluxo 08 - Importacao CSV

Objetivo
Permitir importar respostas de muitos alunos por CSV sem cadastrar tudo manualmente.

Quando implementar
Depois do MVP manual, correcao automatica e revisao basica.

Usuario principal
Professor.

Fluxo funcional
1. Professor abre uma avaliacao.
2. Clica em importar respostas via CSV.
3. Seleciona arquivo CSV.
4. Sistema valida o arquivo.
5. Sistema identifica colunas de aluno e respostas.
6. Sistema cria submissoes.
7. Sistema cria StudentAnswers.
8. Sistema executa correcao em lote.
9. Sistema exibe resumo da importacao.

Formato sugerido
student_name | student_email | q1 | q2 | q3 | q4
Joao         | joao@email.com | B  | 10 | fotossintese | C
Maria        | maria@email.com| A  | 9.8| fotossintese | C

Endpoint recomendado
POST /api/assignments/{assignmentId}/submissions/import

Formato atual:
- CSV

Excel:
- Importacao XLSX ainda nao implementada.
- ClosedXML deve ser usado apenas se a importacao XLSX for adicionada.

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
- Validacao de arquivo

Leitura de CSV:
- CsvHelper

Leitura de Excel:
- Nao implementada no fluxo atual
- ClosedXML deve ser usado se a importacao XLSX for adicionada no futuro
- EPPlus, observando licenca

Banco:
- EF Core
- Insercao em lote
- Transacao

Frontend:
- Angular upload component
- FormData
- HttpClient
- Tela de resumo de importacao

Validacoes
- Arquivo vazio
- Extensao invalida
- MIME type invalido
- Colunas obrigatorias ausentes
- Questao inexistente
- Resposta numerica invalida
- Linha duplicada
- Erros parciais por linha

Regra importante
Nao comece por importacao.
Primeiro faca a submissao manual funcionar.
