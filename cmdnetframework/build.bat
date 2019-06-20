@echo off
cd /d %~dp0
if not exist nuget.exe (
    echo nuget.exe is not fount. Please download from https://www.nuget.org/downloads
    pause
    exit /B
)
cd /d "C:\Program Files (x86)\Microsoft Visual Studio\"
dir /s /b /o:-n MSBuild.exe
for /F "delims=;" %%a in ('dir /s /b /o:-n MSBuild.exe') do (
    set x=%%a
    goto :next
)
:next
if not exist "%x%" (
    echo msbuild.exe is not found. Please install Visual Studio.
    pause
    exit /B
)
cd /d %~dp0
nuget restore ../Implem.SupportTools/Launcher/Implem.SupportTools.csproj /SolutionDirectory ../
nuget restore ../Implem.CodeDefiner.NetFramework/Implem.CodeDefiner.NetFramework.csproj /SolutionDirectory ../
nuget restore ../Implem.Pleasanter.NetFramework/Implem.Pleasanter.NetFramework.csproj /SolutionDirectory ../
"%x%" ../Implem.SupportTools/Launcher/Implem.SupportTools.csproj -property:Configuration=Release /property:OutputPath="../../publish/Implem.SupportTools/" /target:Rebuild -clp:ErrorsOnly
"%x%" ../Implem.CodeDefiner.NetFramework/Implem.CodeDefiner.NetFramework.csproj -property:Configuration=Release /property:OutputPath="../publish/Implem.CodeDefiner/" /target:Rebuild -clp:ErrorsOnly
"%x%" ../Implem.Pleasanter.NetFramework/Implem.Pleasanter.NetFramework.csproj -property:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=../../../cmdnetframework/FolderProfile.xml -clp:ErrorsOnly
xcopy /S /Y ..\Implem.Pleasanter.NetFramework\bin\Release\Publish ..\publish\Implem.Pleasanter\
pause
