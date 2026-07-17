GradeFlow - Roadmap de Implementacao

Objetivo
Organizar a ordem real de desenvolvimento do projeto.

Fase 1 - MVP tecnico
1. Criar solution .NET.
2. Criar projetos Api, Application, Domain e Infrastructure.
3. Configurar EF Core.
4. Criar entidades Assignment, Question, AnswerKey, Submission, StudentAnswer e CorrectionResult.
5. Criar migrations.
6. Criar CRUD basico de Assignment.
7. Criar CRUD basico de Question com AnswerKey.
8. Criar endpoint de Submission manual.
9. Criar CorrectionService.
10. Criar strategies de correcao.
11. Criar endpoint para corrigir submissao.
12. Exibir resultado da correcao.

Fase 2 - Frontend MVP
1. Criar projeto Angular.
2. Criar layout simples.
3. Criar tela de avaliacoes.
4. Criar tela de questoes.
5. Criar tela de gabarito.
6. Criar tela de submissao manual.
7. Criar tela de resultado.
8. Integrar Angular com API.

Fase 3 - Qualidade
1. Testar MultipleChoiceCorrectionStrategy.
2. Testar TrueFalseCorrectionStrategy.
3. Testar NumericCorrectionStrategy.
4. Testar ShortTextCorrectionStrategy.
5. Testar CorrectionService.
6. Testar fluxo completo de submissao e correcao.

Fase 4 - Revisao e auditoria
1. Criar tela de revisao manual.
2. Criar endpoint de revisao.
3. Criar tabela CorrectionLog.
4. Registrar alteracoes de nota.
5. Recalcular nota final apos revisao.

Fase 5 - Autenticacao
1. Criar entidade User.
2. Implementar registro.
3. Implementar login.
4. Implementar JWT.
5. Implementar roles.
6. Proteger endpoints.
7. Criar guards no Angular.
8. Criar interceptor HTTP.
9. Implementar alteracao de senha autenticada.

Fase 5.1 - Preparacao DevOps local
1. Criar CI com GitHub Actions.
2. Adicionar health check.
3. Configurar CORS por ambiente.
4. Containerizar API, frontend e SQL Server local.
5. Definir estrategia controlada de migrations.

Fase 6 - Produtividade do professor
Status: implementada para importacao CSV, relatorios, exportacao CSV, exportacao Excel e exportacao PDF.
1. Importar CSV.
2. Importar Excel futuramente, se XLSX for necessario.
3. Exportar resultado em CSV e Excel.
4. Exportar relatorio em PDF.
5. Criar relatorios basicos.

Fase 7 - Deploy publico final
1. Escolher hospedagem.
2. Configurar secrets.
3. Aplicar migrations de forma controlada.
4. Publicar backend.
5. Publicar frontend.
6. Validar ambiente publico.

Fase 8 - Expansao academica
1. Criar Course.
2. Criar Classroom.
3. Criar Student.
4. Vincular avaliacao a turma.
5. Ver historico do aluno.

Fase 9 - Recursos avancados
1. Questao multi-selecao.
2. Pontuacao parcial.
3. Texto com similaridade.
4. Questao discursiva com rubrica.
5. Upload de arquivos.
6. Extracao de texto.
7. IA assistiva.
8. OCR.

Tecnologias principais do projeto
Backend:
- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core

Frontend:
- Angular
- TypeScript
- Reactive Forms
- HttpClient

Banco:
- SQL Server

Testes:
- xUnit
- FluentAssertions
- Moq, se necessario

Exportacao:
- ClosedXML para Excel
- QuestPDF para PDF

Importacao:
- Leitura simples nativa para CSV basico
- ClosedXML para Excel, se importacao XLSX for implementada

Arquitetura:
- Clean Architecture simples
- Strategy Pattern para correcao
- Services para casos de uso
- Controllers finos

Decisao mais importante
Construa primeiro o motor de correcao.
Todo o resto deve girar em torno dele.
