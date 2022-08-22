FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update && apt-get install -y libgdiplus curl

WORKDIR /src
COPY ["Implem.PleasanterTest/Implem.PleasanterTest.csproj", "Implem.PleasanterTest/"]
COPY ["Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj", "Implem.DefinitionAccessor/"]
COPY ["Implem.ParameterAccessor/Implem.ParameterAccessor.csproj", "Implem.ParameterAccessor/"]
COPY ["Implem.DisplayAccessor/Implem.DisplayAccessor.csproj", "Implem.DisplayAccessor/"]
COPY ["Implem.Libraries/Implem.Libraries.csproj", "Implem.Libraries/"]
COPY ["Rds/Implem.IRds/Implem.IRds.csproj", "Rds/Implem.IRds/"]
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
COPY ["Implem.Plugins/Implem.Plugins.csproj", "Implem.Plugins/"]
COPY ["Implem.Factory/Implem.Factory.csproj", "Implem.Factory/"]
COPY ["Rds/Implem.PostgreSql/Implem.PostgreSql.csproj", "Rds/Implem.PostgreSql/"]
COPY ["Rds/Implem.SqlServer/Implem.SqlServer.csproj", "Rds/Implem.SqlServer/"]
RUN dotnet restore Implem.PleasanterTest
COPY . .
RUN curl -o /usr/bin/jq -L https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64 && chmod +x /usr/bin/jq && \
    cat Implem.Pleasanter/App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL"' > /tmp/Rds.json && \
    cp /tmp/Rds.json Implem.Pleasanter/App_Data/Parameters/Rds.json
RUN dotnet build --no-restore Implem.PleasanterTest

FROM build AS testrunner
WORKDIR /src/Implem.PleasanterTest
CMD ["dotnet", "test", "--no-restore"]
