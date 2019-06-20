Set-Location (Split-Path $MyInvocation.MyCommand.Path -parent)
$nuget = "../cmdnetframework/nuget.exe"
if(-not(Test-Path $nuget))
{
    Write-Host "nuget.exe is not fount. Please download from https://www.nuget.org/downloads"
    exit
}
$msbuild = (Get-ChildItem "C:\Program Files (x86)\Microsoft Visual Studio" -Recurse -Filter "MSBuild.exe").FullName | Sort-Object {$_.Name} | Select-Object -First 1
if ($msbuild -eq $null)
{
    Write-Host "msbuild.exe is not found. Please install Visual Studio."
    exit
}
& $nuget "restore" "../Implem.SupportTools/Launcher/Implem.SupportTools.csproj" "/SolutionDirectory" "../"
& $nuget "restore" "../Implem.CodeDefiner.NetFramework/Implem.CodeDefiner.NetFramework.csproj" "/SolutionDirectory" "../"
& $nuget "restore" "../Implem.Pleasanter.NetFramework/Implem.Pleasanter.NetFramework.csproj" "/SolutionDirectory" "../"
if(Test-Path "SetupTmp")
{
    Remove-Item "SetupTmp" -Recurse
}
& dotnet "publish" "../Implem.CodeDefiner.NetCore/Implem.CodeDefiner.NetCore.csproj" "-c" "Release" "-v" "q" "-o" "../cmdsetup/SetupTmp/Pleasanter/CodeDefiner.NetCore/"
& dotnet "publish" "../Implem.Pleasanter.NetCore/Implem.Pleasanter.NetCore.csproj" "-c" "Release" "-v" "q" "-o" "../cmdsetup/SetupTmp/Pleasanter/Pleasanter.NetCore/"
& $msbuild "../Implem.Pleasanter.NetFramework/Implem.Pleasanter.NetFramework.csproj" "-p:Configuration=Release;DeployIISAppPath=Default Web Site/pleasanter;DeployOnBuild=true;WebPublishMethod=Package;PackageAsSingleFile=true;SkipInvalidConfigurations=true;PackageLocation=../cmdsetup/SetupTmp/Pleasanter/Pleasanter.NetFramework/" "/target:Rebuild" "-clp:ErrorsOnly"
& $msbuild "../Implem.CodeDefiner.NetFramework/Implem.CodeDefiner.NetFramework.csproj" "-p:Configuration=Release;SkipInvalidConfigurations=true;OutputPath=../cmdsetup/SetupTmp/Pleasanter/CodeDefiner.NetFramework/" "/target:Rebuild" "-clp:ErrorsOnly"
& $msbuild "../Implem.SupportTools/Launcher/Implem.SupportTools.csproj" "-p:Configuration=Release;SkipInvalidConfigurations=true;OutputPath=../../cmdsetup/SetupTmp/Pleasanter/SupportTools/" "/target:Rebuild" "-clp:ErrorsOnly"
Copy-Item "Readme.txt" "./SetupTmp/Readme.txt"
Copy-Item "pleasanter.sh" "./SetupTmp/Pleasanter/pleasanter.sh"
Copy-Item "Setup.bat" "./SetupTmp/Pleasanter/Setup.bat"
Copy-Item "Setup.sh" "./SetupTmp/Pleasanter/Setup.sh"
Copy-Item "../README_PLEASANTER.md" "./SetupTmp/Pleasanter/Readme.md"
Copy-Item "../LICENSES" "./SetupTmp/Pleasanter" -Recurse
$zipFileName = ("Pleasanter" + (Get-ItemProperty "../Implem.Pleasanter.NetFramework/bin/Implem.Pleasanter.NetFramework.dll").VersionInfo.FileVersion)
Rename-Item "SetupTmp" $zipFileName
if(Test-Path ($zipFileName + ".zip"))
{
    Remove-Item ($zipFileName + ".zip")
}
Compress-Archive -Path $zipFileName -DestinationPath ($zipFileName + ".zip")
Remove-Item $zipFileName -Recurse
