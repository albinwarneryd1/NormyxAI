FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Sylvaro.sln .
COPY src/Sylvaro.Api/Sylvaro.Api.csproj src/Sylvaro.Api/
COPY src/Sylvaro.Application/Sylvaro.Application.csproj src/Sylvaro.Application/
COPY src/Sylvaro.Domain/Sylvaro.Domain.csproj src/Sylvaro.Domain/
COPY src/Sylvaro.Infrastructure/Sylvaro.Infrastructure.csproj src/Sylvaro.Infrastructure/
RUN dotnet restore src/Sylvaro.Api/Sylvaro.Api.csproj

COPY . .
RUN dotnet publish src/Sylvaro.Api/Sylvaro.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
COPY policy-packs ./policy-packs
COPY prompts ./prompts
COPY reference-notes ./reference-notes
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sylvaro.Api.dll"]
