#!/bin/bash
mkdir -p /var/www/pleasanter/
cp -r Pleasanter.NetCore/* /var/www/pleasanter/
dotnet CodeDefiner.NetCore/Implem.CodeDefiner.NetCore.dll _rds /p \\var\\www\\pleasanter /s /r
