GradeFlow - Roadmap de Implementacao

Objetivo
Organizar a ordem real de desenvolvimento do projeto.

Fase 1 - MVP técnico
1. Criar solution .NET.
2. Criar projetos Api, Application, Domain e Infrastructure.
3. Configurar EF Core.
4. Criar entidades Assignment, Question, AnswerKey, Submission, StudentAnswer e CorrectionResult.
5. Criar migrations.
6. Criar CRUD básico de Assignment.
7. Criar CRUD básico de Question com AnswerKey.
8. Criar endpoint de Submission manual.
9. Criar CorrectionService.
10. Criar strategies de correção.
11. Criar endpoint para corrigir submissão.
12. Exibir resultado da correção.

Fase 2 - Frontend MVP
1. Criar projeto Angular.
2. Criar layout simples.
3. Criar tela de avaliações.
4. Criar tela de questões.
5. Criar tela de gabarito.
6. Criar tela de submissão manual.
7. Criar tela de resultado.
8. Integrar Angular com API.

Fase 3 - Qualidade
1. Testar MultipleChoiceCorrectionStrategy.
2. Testar TrueFalseCorrectionStrategy.
3. Testar NumericCorrectionStrategy.
4. Testar ShortTextCorrectionStrategy.
5. Testar CorrectionService.
6. Testar fluxo completo de submissão e correção.

Fase 4 - Revisão e auditoria
1. Criar tela de revisão manual.
2. Criar endpoint de revisão.
3. Criar tabela CorrectionLog.
4. Registrar alterações de nota.
5. Recalcular nota final após revisão.

Fase 5 - autenticação
1. Criar entidade User.
2. Implementar registro.
3. Implementar login.
4. Implementar JWT.
5. Implementar roles.
6. Proteger endpoints.
7. Criar guards no Angular.
8. Criar interceptor HTTP.
9. Implementar alteração de senha autenticada.

Fase 5.1 - Preparação DevOps Local
1. Criar CI com GitHub Actions.
2. Adicionar health check.
3. Configurar CORS por ambiente.
4. Containerizar API, frontend e SQL Server local.
5. Definir estratégia controlada de migrations.

Fase 6 - Produtividade do professor
Status: implementada para importação CSV, relatórios, exportação CSV, exportação Excel e exportação PDF.
1. Importar CSV.
2. Importar Excel futuramente, se XLSX for necessário.
3. Exportar resultado em CSV e Excel.
4. Exportar relatório em PDF.
5. Criar relatórios básicos.

Fase 7 - Deploy público final
1. Escolher hospedagem.
2. Configurar secrets.
3. Aplicar migrations de forma controlada.
4. Publicar backend.
5. Publicar frontend.
6. Validar ambiente público.

Fase 8 - Expansao acadêmica
1. Criar Course.
2. Criar Classroom.
3. Criar Student.
4. Vincular avaliação a turma.
5. Ver histórico do aluno.

Fase 9 - Recursos avançados
1. questão multi-seleção.
2. pontuação parcial.
3. Texto com similaridade.
4. questão discursiva com rubrica.
5. Upload de arquivos.
6. Extração de texto.
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
- Moq, se necessário

Exportação:
- ClosedXML para Excel
- QuestPDF para PDF

Importação:
- Leitura simples nativa para CSV básico
- ClosedXML para Excel, se importação XLSX for implementada

Arquitetura:
- Clean Architecture simples
- Strategy Pattern para correção
- Services para casos de uso
- Controllers finos

decisão mais importante
Construa primeiro o motor de correção.
Todo o resto deve girar em torno dele.
