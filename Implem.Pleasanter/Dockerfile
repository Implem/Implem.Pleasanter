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
WORKDIR /src
COPY Implem.Pleasanter/Implem.Pleasanter.csproj Implem.Pleasanter/
COPY Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj Implem.DefinitionAccessor/
COPY Implem.ParameterAccessor/Implem.ParameterAccessor.csproj Implem.ParameterAccessor/
COPY Implem.DisplayAccessor/Implem.DisplayAccessor.csproj Implem.DisplayAccessor/
COPY Implem.Libraries/Implem.Libraries.csproj Implem.Libraries/
COPY Rds/Implem.IRds/Implem.IRds.csproj Rds/Implem.IRds/
COPY Implem.Plugins/Implem.Plugins.csproj Implem.Plugins/
COPY Implem.Factory/Implem.Factory.csproj Implem.Factory/
COPY Rds/Implem.PostgreSql/Implem.PostgreSql.csproj Rds/Implem.PostgreSql/
COPY Rds/Implem.SqlServer/Implem.SqlServer.csproj Rds/Implem.SqlServer/
COPY Rds/Implem.MySql/Implem.MySql.csproj Rds/Implem.MySql/
RUN dotnet restore "Implem.Pleasanter/Implem.Pleasanter.csproj"
COPY . .
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet build "Implem.Pleasanter.csproj" -c Release -o /app/build  -p:SkipBuildTask=true \
 && dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish -p:SkipBuildTask=true

FROM build AS publish
RUN apt-get update && apt-get install -y \
    jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

# RUN dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish -p:SkipBuildTask=true
RUN cat App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL" | .SaConnectionString|=null | .OwnerConnectionString|=null | .UserConnectionString|=null' \
    > /app/publish/App_Data/Parameters/Rds.json

FROM publish AS settings
RUN cat App_Data/Parameters/Service.json \
    | jq '.TimeZoneDefault|="Asia/Tokyo" | .DefaultLanguage|="ja" | .WithoutChangeDefaultPassword|=true | .ShowStartGuide|=false | .Demo|=false' \
    > /app/publish/App_Data/Parameters/Service.json
RUN cat App_Data/Parameters/User.json \
    | jq '.Theme|="sunny"' \
    > /app/publish/App_Data/Parameters/User.json

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM base AS testing
COPY --from=settings /app/publish .
COPY --from=frontend-build /app/dist /Implem.Pleasanter/wwwroot wwwroot/
ENTRYPOINT ["dotnet", "Implem.Pleasanter.dll"]

FROM base AS final
COPY --from=publish /app/publish .
COPY --from=frontend-build /app/dist /Implem.Pleasanter/wwwroot wwwroot/
ENTRYPOINT ["dotnet", "Implem.Pleasanter.dll"]
