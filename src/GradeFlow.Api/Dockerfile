FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GradeFlow.slnx ./
COPY Directory.Build.props ./
COPY src/GradeFlow.Api/GradeFlow.Api.csproj src/GradeFlow.Api/
COPY src/GradeFlow.Application/GradeFlow.Application.csproj src/GradeFlow.Application/
COPY src/GradeFlow.Domain/GradeFlow.Domain.csproj src/GradeFlow.Domain/
COPY src/GradeFlow.Infrastructure/GradeFlow.Infrastructure.csproj src/GradeFlow.Infrastructure/
RUN dotnet restore src/GradeFlow.Api/GradeFlow.Api.csproj

COPY src/ src/
RUN dotnet publish src/GradeFlow.Api/GradeFlow.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GradeFlow.Api.dll"]
