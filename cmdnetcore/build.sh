#!/bin/bash
cd `dirname $0`
dotnet publish ../Implem.CodeDefiner.NetCore/Implem.CodeDefiner.NetCore.csproj -c Release -v q -o ../publish/Implem.CodeDefiner/ | grep -v warning
dotnet publish ../Implem.Pleasanter.NetCore/Implem.Pleasanter.NetCore.csproj -c Release -v q -o ../publish/Implem.Pleasanter/ | grep -v warning

