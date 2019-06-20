cd /d %~dp0
call .\Pleasanter.NetFramework\Implem.Pleasanter.NetFramework.deploy.cmd /T /Y
call .\CodeDefiner.NetFramework\Implem.CodeDefiner.NetFramework.exe _rds /p %SystemDrive%\inetpub\wwwroot\pleasanter /s /r
pause