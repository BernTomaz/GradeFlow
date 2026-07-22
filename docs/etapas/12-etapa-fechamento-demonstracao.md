# GradeFlow - Etapa 12 - Fechamento para Demonstração

## Objetivo

Preparar o GradeFlow para uma demonstração pública do MVP, sem adicionar funcionalidades grandes antes do deploy.

Esta etapa existe para polir o que já está pronto, reduzir riscos e deixar o projeto apresentável.

## Pré-requisitos

- Etapas 01 a 11 concluídas.
- Backend compilando e testes passando.
- Frontend compilando.
- Fluxo principal funcionando:
  - cadastro;
  - login;
  - criação de avaliação;
  - criação de questões e gabaritos;
  - submissão de respostas;
  - correção automática;
  - revisão manual;
  - importação CSV;
  - relatórios e exportações.

## Escopo Recomendado

Implementar apenas melhorias pequenas e úteis para demonstrar o produto.

### Dashboard Administrativo

Status: implementado no frontend como tela inicial após login.

A tela inicial possui:

- total de avaliações;
- total de alunos avaliados;
- média geral das avaliações corrigidas;
- quantidade de respostas pendentes de revisão;
- gráfico de acertos por questão em barras, colunas ou pizza;
- página própria de relatório por avaliação;
- exportação do dashboard em PDF via impressão do navegador;
- atalhos para avaliações, submissões e relatórios.

Não criar gráficos complexos nesta etapa.

### Ajustes de Experiência

Revisar:

- mensagens de erro;
- estados vazios;
- botões principais;
- navegação após criar, corrigir ou revisar;
- carregamento de telas;
- responsividade básica.

### Dados de Demonstração

Definir uma forma segura de demonstrar o sistema:

- usuário professor de demonstração, se necessário;
- avaliação exemplo;
- questões exemplo;
- submissão exemplo;
- resultado corrigido;
- revisão manual registrada.

Não versionar senhas reais.

### Documentação Final do MVP

Atualizar:

- `README.md`;
- `docs/status-projeto.md`;
- `docs/endpoints.md`, se houver mudança de endpoint;
- `docs/configuracao.md`, se houver mudança de execução;
- prints ou instruções de demonstração, se forem adicionados.

## Fora do Escopo

Não implementar nesta etapa:

- upload de arquivos;
- OCR;
- IA;
- turmas;
- cursos;
- cadastro completo de alunos;
- importação XLSX;
- refresh token persistente;
- recuperação de senha;
- deploy público.

Esses itens ficam para etapas futuras.

## Validação

Antes de encerrar a etapa:

```powershell
dotnet test GradeFlow.slnx --no-restore -m:1
```

```powershell
cd src\GradeFlow.Web
npm run build
```

Validar manualmente:

- login;
- CRUD de avaliação;
- CRUD de questão;
- submissão;
- correção;
- revisão;
- importação CSV;
- exportação CSV, Excel e PDF;
- dashboard;
- exportação em PDF do dashboard pela impressão do navegador.

## Critérios de Aceite

- O MVP pode ser demonstrado de ponta a ponta.
- A tela inicial orienta o professor para os fluxos principais.
- Não há funcionalidade grande parcialmente implementada.
- Documentação principal está atualizada.
- Build e testes passam.

## Commit Sugerido

```text
feat: polish MVP demonstration flow
```
