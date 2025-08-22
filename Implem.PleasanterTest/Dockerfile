FROM node:22-alpine AS frontend-build
WORKDIR /app
# Copy package files for dependency installation
COPY Implem.PleasanterFrontend/wwwroot/package*.json ./
RUN npm ci
RUN npm audit --audit-level high || true
# Copy all necessary files for frontend build
COPY Implem.PleasanterFrontend/wwwroot/ ./
RUN npm run build -- --outDir=/app/dist/assets

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && apt-get install -y \
    jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY Implem.PleasanterTest/Implem.PleasanterTest.csproj Implem.PleasanterTest/
COPY Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj Implem.DefinitionAccessor/
COPY Implem.ParameterAccessor/Implem.ParameterAccessor.csproj Implem.ParameterAccessor/
COPY Implem.DisplayAccessor/Implem.DisplayAccessor.csproj Implem.DisplayAccessor/
COPY Implem.Libraries/Implem.Libraries.csproj Implem.Libraries/
COPY Implem.Pleasanter/Implem.Pleasanter.csproj Implem.Pleasanter/
COPY Rds/Implem.IRds/Implem.IRds.csproj Rds/Implem.IRds/
COPY Implem.Plugins/Implem.Plugins.csproj Implem.Plugins/
COPY Implem.Factory/Implem.Factory.csproj Implem.Factory/
COPY Rds/Implem.PostgreSql/Implem.PostgreSql.csproj Rds/Implem.PostgreSql/
COPY Rds/Implem.SqlServer/Implem.SqlServer.csproj Rds/Implem.SqlServer/
COPY Rds/Implem.MySql/Implem.MySql.csproj Rds/Implem.MySql/
RUN dotnet restore Implem.PleasanterTest
COPY . .
RUN cat Implem.Pleasanter/App_Data/Parameters/Rds.json | jq '.Dbms|="PostgreSQL"' > /tmp/Rds.json \
 && cp /tmp/Rds.json Implem.Pleasanter/App_Data/Parameters/Rds.json \
 && rm -f /tmp/Rds.json
COPY --from=frontend-build /app/dist /Implem.Pleasanter/wwwroot Implem.Pleasanter/wwwroot/

RUN dotnet build --no-restore Implem.PleasanterTest -p:SkipBuildTask=true

FROM build AS testrunner
WORKDIR /src/Implem.PleasanterTest
ENTRYPOINT ["dotnet", "test", "--no-restore", "-p:SkipBuildTask=true"]
