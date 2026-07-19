GradeFlow - Fluxo 09 - Upload de Arquivos, IA e OCR

Objetivo
Evoluir o sistema para aceitar arquivos anexados e correção assistida por IA.

Quando implementar
Somente depois do MVP, autenticação, importação e revisão manual estarem funcionando bem.

Fluxo de upload
1. Professor ou aluno envia arquivo.
2. Sistema valida extensão, tamanho e MIME type.
3. Sistema armazena arquivo.
4. Sistema registra metadados.
5. Sistema tenta extrair texto, se aplicavel.
6. Sistema envia para revisão ou correção assistida.

Campos sugeridos
UploadedFile:
- Id
- SubmissionId
- UploadedFileUrl
- FileName
- FileType
- FileSize
- ExtractedText
- ExtractionStatus
- CreatedAt

Tecnologias para upload
Backend:
- ASP.NET Core IFormFile
- validação de MIME type
- Limite de tamanho

Armazenamento:
- Disco local no desenvolvimento
- Azure Blob Storage, AWS S3 ou similar em produção

Extração de texto
PDF pesquisavel:
- PdfPig
- iText, observando licenca

DOCX:
- Open XML SDK

OCR:
- Tesseract OCR
- Azure Computer Vision
- Google Cloud Vision
- AWS Textract

IA
Usos recomendados:
- Sugerir feedback
- Identificar conceitos ausentes
- Comparar resposta com rubrica
- Classificar resposta discursiva
- Sugerir pontuação

Cuidados com IA
- IA deve sugerir, não decidir sozinha.
- Respostas discursivas precisam de revisão humana.
- OCR pode errar caracteres.
- Escrita manual é muito mais difícil que texto digitado.

Frontend:
- Angular upload component
- Barra de progresso
- Exibição do texto extraido
- Tela de revisão da sugestão da IA

Regra importante
IA e OCR não devem entrar no MVP.
Sem motor de correção confiável, IA vira enfeite caro.

