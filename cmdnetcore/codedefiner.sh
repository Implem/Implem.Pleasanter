#!/bin/bash
cd `dirname $0`
dotnet ../publish/Implem.CodeDefiner/Implem.CodeDefiner.NetCore.dll _rds
