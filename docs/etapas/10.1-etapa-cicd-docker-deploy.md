GradeFlow - Etapa 13 - CI/CD, Docker e Deploy

Objetivo
Automatizar build, testes e execucao do projeto, e deixar o sistema acessivel publicamente para avaliacao externa.

Quando implementar
Depois que login e permissoes (etapa 10) estiverem funcionando.
Nao faz sentido publicar um sistema sem controle de acesso minimo.

Tecnologias
CI/CD:
- GitHub Actions

Containerizacao:
- Docker
- Docker Compose

Deploy:
- Backend: Azure App Service, Railway ou Render
- Frontend: Vercel, Netlify ou Azure Static Web Apps
- Banco: Azure SQL, Railway Postgres ou SQL Server em container

Docker
Tarefas:
- Criar Dockerfile para GradeFlow.Api
- Criar Dockerfile para GradeFlow.Web
- Criar docker-compose.yml com Api, Web e banco de dados
- Configurar variaveis de ambiente via appsettings ou .env
- Garantir que migrations rodem automaticamente ao subir o container

CI (Integracao Continua)
Workflow: .github/workflows/ci.yml

Deve rodar em todo push e pull request:
- Restore e build do backend (.NET)
- Execucao dos testes (dotnet test)
- Build do frontend (npm ci, npm run build)
- Falhar o pipeline se testes ou build quebrarem

CD (Entrega Continua)
Workflow: .github/workflows/deploy.yml

Deve rodar apenas na branch main, depois do CI passar:
- Build da imagem Docker da Api
- Build da imagem Docker do Web
- Push para o registry (Docker Hub ou GitHub Container Registry)
- Deploy automatico no servico escolhido

Deploy publico
Tarefas:
- Publicar backend com HTTPS valido
- Publicar frontend apontando para a URL publica da Api
- Ajustar CORS da Api para o dominio publico do frontend
- Adicionar link do ambiente publico no README

Ponto de atencao
Nao suba appsettings com senha ou connection string real no repositorio.
Use secrets do GitHub Actions e variaveis de ambiente no servico de deploy.
Sem isso, nenhuma das etapas anteriores fica visivel para quem nao clona o projeto.
