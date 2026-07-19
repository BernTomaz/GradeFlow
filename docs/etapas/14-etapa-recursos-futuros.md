# GradeFlow - Etapa 14 - Recursos Futuros

## Objetivo

Organizar evoluções futuras sem misturar com o MVP, com o fechamento para demonstração ou com o deploy público.

Esta etapa só deve começar depois que o projeto estiver demonstrável e publicado, ou quando houver uma decisão clara de expandir o produto.

## Regra Principal

Escolha um recurso por vez.

Não implemente IA, OCR, upload, turmas e importação avançada na mesma etapa.

## Backlog Futuro

### Acadêmico

- Turmas.
- Cursos.
- Cadastro completo de alunos.
- Histórico por aluno.
- Vínculo entre avaliação e turma.

### Questões e Correção

- Questões de multi-seleção.
- Pontuação parcial.
- Texto com similaridade.
- Questões discursivas com rubrica.
- Recorreção em lote.

### Importação e Arquivos

- Importação XLSX.
- Upload de arquivos.
- Extração de texto de PDF e DOCX.

### IA e OCR

- OCR.
- IA assistiva.
- Sugestão de feedback.
- Comparação com rubrica.
- Identificação de conceitos ausentes.
- Sugestão de pontuação.

## Tecnologias Possíveis

### Upload

- ASP.NET Core `IFormFile`.
- Azure Blob Storage.
- AWS S3.

### PDF

- PdfPig.
- QuestPDF.

### DOCX

- Open XML SDK.

### OCR

- Tesseract.
- Azure Computer Vision.
- Google Cloud Vision.
- AWS Textract.

### IA

- Sugestão de feedback.
- Comparação com rubrica.
- Identificação de conceitos ausentes.
- Sugestão de pontuação.

## Regra para IA

IA deve sugerir, não decidir sozinha.

Respostas discursivas precisam continuar revisáveis por professor.

## Critérios Antes de Implementar

Antes de iniciar qualquer recurso futuro, responder:

- Isso melhora a demonstração ou o uso real?
- Existe fluxo manual funcionando antes da automação?
- O banco precisa mudar?
- O frontend precisa mudar?
- Quais testes mínimos protegem a mudança?
- O recurso pode ser entregue em uma etapa pequena?

## Ponto de Atenção

Esses recursos são bons, mas podem destruir o foco se entrarem cedo demais.

O caminho recomendado é publicar o MVP primeiro e evoluir depois.
