#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y libgdiplus curl

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
COPY ["Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj", "Implem.DefinitionAccessor/"]
COPY ["Implem.ParameterAccessor/Implem.ParameterAccessor.csproj", "Implem.ParameterAccessor/"]
COPY ["Implem.DisplayAccessor/Implem.DisplayAccessor.csproj", "Implem.DisplayAccessor/"]
COPY ["Implem.Libraries/Implem.Libraries.csproj", "Implem.Libraries/"]
COPY ["Rds/Implem.IRds/Implem.IRds.csproj", "Rds/Implem.IRds/"]
COPY ["Implem.Factory/Implem.Factory.csproj", "Implem.Factory/"]
COPY ["Rds/Implem.PostgreSql/Implem.PostgreSql.csproj", "Rds/Implem.PostgreSql/"]
COPY ["Rds/Implem.SqlServer/Implem.SqlServer.csproj", "Rds/Implem.SqlServer/"]
RUN dotnet restore "Implem.Pleasanter/Implem.Pleasanter.csproj"
COPY . .
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet build "Implem.Pleasanter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish
RUN curl -o /usr/bin/jq -L https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64 && chmod +x /usr/bin/jq && \
    cat App_Data/Parameters/Service.json \
    | jq '.WithoutChangeDefaultPassword|=true | .ShowStartGuide|=false | .Demo|=false' \
    > /app/publish/App_Data/Parameters/Service.json
RUN cat App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL"' > /app/publish/App_Data/Parameters/Rds.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Implem.Pleasanter.dll"]
