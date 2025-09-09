# Use a imagem oficial do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copia csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o restante do projeto
COPY . ./

# Build da aplicação
RUN dotnet publish -c Release -o out

# Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia a build
COPY --from=build /app/out .

# Porta padrão
EXPOSE 8080

# Variável de ambiente para o Render (substitua se quiser outro nome)
ENV ASPNETCORE_URLS=http://+:8080

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "framework-backend.dll"]
