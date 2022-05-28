#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update && apt-get install -y curl

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Implem.CodeDefiner/Implem.CodeDefiner.csproj", "Implem.CodeDefiner/"]
COPY ["Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj", "Implem.DefinitionAccessor/"]
COPY ["Implem.ParameterAccessor/Implem.ParameterAccessor.csproj", "Implem.ParameterAccessor/"]
COPY ["Implem.DisplayAccessor/Implem.DisplayAccessor.csproj", "Implem.DisplayAccessor/"]
COPY ["Implem.Libraries/Implem.Libraries.csproj", "Implem.Libraries/"]
COPY ["Rds/Implem.IRds/Implem.IRds.csproj", "Rds/Implem.IRds/"]
COPY ["Implem.Factory/Implem.Factory.csproj", "Implem.Factory/"]
COPY ["Rds/Implem.PostgreSql/Implem.PostgreSql.csproj", "Rds/Implem.PostgreSql/"]
COPY ["Rds/Implem.SqlServer/Implem.SqlServer.csproj", "Rds/Implem.SqlServer/"]
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
RUN dotnet restore "Implem.CodeDefiner/Implem.CodeDefiner.csproj"
COPY . .
WORKDIR "/src/Implem.CodeDefiner"
RUN dotnet build "Implem.CodeDefiner.csproj" -c Release -o /app/build/Implem.CodeDefiner
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet build "Implem.Pleasanter.csproj" -c Release -o /app/build/Implem.Pleasanter

FROM build AS publish
WORKDIR "/src/Implem.CodeDefiner"
RUN dotnet publish "Implem.CodeDefiner.csproj" -c Release -o /app/publish/Implem.CodeDefiner
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish/Implem.Pleasanter
RUN curl -o /usr/bin/jq -L https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64 && chmod +x /usr/bin/jq && \
    cat App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL"' > /app/publish/Implem.Pleasanter/App_Data/Parameters/Rds.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /app/Implem.CodeDefiner

ENTRYPOINT ["dotnet", "Implem.CodeDefiner.dll"]