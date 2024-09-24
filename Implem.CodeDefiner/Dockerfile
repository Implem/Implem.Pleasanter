#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
COPY ["Rds/Implem.MySql/Implem.MySql.csproj", "Rds/Implem.MySql/"]
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
RUN dotnet restore "Implem.CodeDefiner/Implem.CodeDefiner.csproj"
COPY . .
RUN dotnet build "Implem.CodeDefiner/Implem.CodeDefiner.csproj" -c Release -o /app/build/Implem.CodeDefiner
RUN dotnet build "Implem.Pleasanter/Implem.Pleasanter.csproj" -c Release -o /app/build/Implem.Pleasanter

FROM build AS publish
RUN apt-get update && apt-get install -y jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

RUN dotnet publish "Implem.CodeDefiner/Implem.CodeDefiner.csproj" -c Release -o /app/publish/Implem.CodeDefiner
RUN dotnet publish "Implem.Pleasanter/Implem.Pleasanter.csproj" -c Release -o /app/publish/Implem.Pleasanter
RUN cat Implem.Pleasanter/App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL" | .SaConnectionString|=null | .OwnerConnectionString|=null | .UserConnectionString|=null' \
    > /app/publish/Implem.Pleasanter/App_Data/Parameters/Rds.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /app/Implem.CodeDefiner

ENTRYPOINT ["dotnet", "Implem.CodeDefiner.dll"]