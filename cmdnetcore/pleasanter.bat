cd /d %~dp0
cd ..\publish\Implem.Pleasanter\
dotnet Implem.Pleasanter.NetCore.dll --urls=http://localhost:5000 --pathbase=/pleasanter
