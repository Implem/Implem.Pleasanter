#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y \
    libgdiplus \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
COPY ["Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj", "Implem.DefinitionAccessor/"]
COPY ["Implem.ParameterAccessor/Implem.ParameterAccessor.csproj", "Implem.ParameterAccessor/"]
COPY ["Implem.DisplayAccessor/Implem.DisplayAccessor.csproj", "Implem.DisplayAccessor/"]
COPY ["Implem.Libraries/Implem.Libraries.csproj", "Implem.Libraries/"]
COPY ["Rds/Implem.IRds/Implem.IRds.csproj", "Rds/Implem.IRds/"]
COPY ["Implem.Plugins/Implem.Plugins.csproj", "Implem.Plugins/"]
COPY ["Implem.Factory/Implem.Factory.csproj", "Implem.Factory/"]
COPY ["Rds/Implem.PostgreSql/Implem.PostgreSql.csproj", "Rds/Implem.PostgreSql/"]
COPY ["Rds/Implem.SqlServer/Implem.SqlServer.csproj", "Rds/Implem.SqlServer/"]
RUN dotnet restore "Implem.Pleasanter/Implem.Pleasanter.csproj"
COPY . .
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet build "Implem.Pleasanter.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update && apt-get install -y \
    jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

RUN dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish
RUN cat App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL" | .SaConnectionString|=null | .OwnerConnectionString|=null | .UserConnectionString|=null' \
    > /app/publish/App_Data/Parameters/Rds.json

FROM publish AS settings
RUN cat App_Data/Parameters/Service.json \
    | jq '.WithoutChangeDefaultPassword|=true | .ShowStartGuide|=false | .Demo|=false' \
    > /app/publish/App_Data/Parameters/Service.json

FROM base AS testing
WORKDIR /app
COPY --from=settings /app/publish .
ENTRYPOINT ["dotnet", "Implem.Pleasanter.dll"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Implem.Pleasanter.dll"]
