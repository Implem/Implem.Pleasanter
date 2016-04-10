cd /d %~dp0
nuget.exe restore Implem.Pleasanter.sln
nuget.exe restore Implem.CodeDefiner.sln
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" Implem.CodeDefiner.sln /t:rebuild /p:Configuration=Release
.\Implem.CodeDefiner\bin\Release\Implem.CodeDefiner.exe _rds
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" Implem.Pleasanter.sln /t:rebuild /p:Configuration=Release /P:DeployOnBuild=true;CreatePackageOnPublish=true;DeployIisAppPath="Default Web Site/pleasanter"
.\Implem.Pleasanter\obj\Release\Package\Implem.Pleasanter.deploy.cmd /T /Y
pause