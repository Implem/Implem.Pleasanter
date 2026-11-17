FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Implem.CodeDefiner/*.csproj Implem.CodeDefiner/
COPY Implem.DefinitionAccessor/*.csproj Implem.DefinitionAccessor/
COPY Implem.ParameterAccessor/*.csproj Implem.ParameterAccessor/
COPY Implem.DisplayAccessor/*.csproj Implem.DisplayAccessor/
COPY Implem.Libraries/*.csproj Implem.Libraries/
COPY Implem.Factory/*.csproj Implem.Factory/
COPY Implem.Plugins/*.csproj Implem.Plugins/
COPY Rds/Implem.IRds/*.csproj Rds/Implem.IRds/
COPY Rds/Implem.PostgreSql/*.csproj Rds/Implem.PostgreSql/
COPY Rds/Implem.SqlServer/*.csproj Rds/Implem.SqlServer/
COPY Rds/Implem.MySql/*.csproj Rds/Implem.MySql/
COPY Implem.Pleasanter/*.csproj Implem.Pleasanter/
COPY Implem.PleasanterFrontend/Implem.PleasanterFrontend.csproj Implem.PleasanterFrontend/
# コンテナの RID (linux-x64 / linux-arm64) だけを対象に restore / build / publish する。
# RID を指定しないと、NuGet パッケージのネイティブ資産 (SkiaSharp など) が全 RID 分 runtimes/ に出力される。
# ベースイメージの .NET ランタイムを使う framework-dependent のままにする (SelfContained=false)。
# dotnet restore には --self-contained がないため -p:SelfContained=false で指定する。
RUN dotnet restore "Implem.CodeDefiner/Implem.CodeDefiner.csproj" --use-current-runtime -p:SelfContained=false \
 && dotnet restore "Implem.Pleasanter/Implem.Pleasanter.csproj" --use-current-runtime -p:SelfContained=false
COPY . .
RUN dotnet build "Implem.CodeDefiner/Implem.CodeDefiner.csproj" -c Release -o /app/build/Implem.CodeDefiner --no-restore -p:SkipBuildTask=true --use-current-runtime --self-contained false \
 && dotnet build "Implem.Pleasanter/Implem.Pleasanter.csproj" -c Release -o /app/build/Implem.Pleasanter --no-restore -p:SkipBuildTask=true --use-current-runtime --self-contained false

FROM build AS publish
RUN apt-get update && apt-get install -y jq \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

RUN dotnet publish "Implem.CodeDefiner/Implem.CodeDefiner.csproj" -c Release -o /app/publish/Implem.CodeDefiner --no-restore -p:SkipBuildTask=true --use-current-runtime --self-contained false \
&& dotnet publish "Implem.Pleasanter/Implem.Pleasanter.csproj" -c Release -o /app/publish/Implem.Pleasanter --no-restore -p:SkipBuildTask=true --use-current-runtime --self-contained false
RUN cat Implem.Pleasanter/App_Data/Parameters/Rds.json \
    | jq '.Dbms|="PostgreSQL" | .SaConnectionString|=null | .OwnerConnectionString|=null | .UserConnectionString|=null' \
    > /app/publish/Implem.Pleasanter/App_Data/Parameters/Rds.json

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

#GSSAPI/Kerberos ライブラリのインストール
RUN apt-get update && \
    apt-get install -y libkrb5-3 libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
WORKDIR /app/Implem.CodeDefiner

ENTRYPOINT ["dotnet", "Implem.CodeDefiner.dll"]
