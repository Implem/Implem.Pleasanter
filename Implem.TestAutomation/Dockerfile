#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Implem.TestAutomation/Implem.TestAutomation.csproj", "Implem.TestAutomation/"]
COPY ["Implem.DefinitionAccessor/Implem.DefinitionAccessor.csproj", "Implem.DefinitionAccessor/"]
COPY ["Implem.ParameterAccessor/Implem.ParameterAccessor.csproj", "Implem.ParameterAccessor/"]
COPY ["Implem.DisplayAccessor/Implem.DisplayAccessor.csproj", "Implem.DisplayAccessor/"]
COPY ["Implem.Libraries/Implem.Libraries.csproj", "Implem.Libraries/"]
COPY ["Rds/Implem.IRds/Implem.IRds.csproj", "Rds/Implem.IRds/"]
COPY ["Implem.Pleasanter/Implem.Pleasanter.csproj", "Implem.Pleasanter/"]
COPY ["Implem.Factory/Implem.Factory.csproj", "Implem.Factory/"]
COPY ["Rds/Implem.PostgreSql/Implem.PostgreSql.csproj", "Rds/Implem.PostgreSql/"]
COPY ["Rds/Implem.SqlServer/Implem.SqlServer.csproj", "Rds/Implem.SqlServer/"]
RUN dotnet restore "Implem.TestAutomation/Implem.TestAutomation.csproj"
COPY . .
WORKDIR "/src/Implem.TestAutomation"
RUN dotnet build "Implem.TestAutomation.csproj" -c Release -o /app/build/Implem.TestAutomation
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet build "Implem.Pleasanter.csproj" -c Release -o /app/build/Implem.Pleasanter

FROM build AS publish
WORKDIR "/src/Implem.TestAutomation"
RUN dotnet publish "Implem.TestAutomation.csproj" -c Release -o /app/publish/Implem.TestAutomation
WORKDIR "/src/Implem.Pleasanter"
RUN dotnet publish "Implem.Pleasanter.csproj" -c Release -o /app/publish/Implem.Pleasanter

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Implem.TestAutomation/Implem.TestAutomation.dll", "/p", "Implem.Pleasanter/Implem.Pleasanter", "/s"]
# ENTRYPOINT ["dotnet", "Implem.TestAutomation/Implem.TestAutomation.dll"]
