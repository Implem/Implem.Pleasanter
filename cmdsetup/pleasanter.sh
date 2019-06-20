#!/bin/bash
cd `dirname $0`
cd /var/www/pleasanter/
dotnet Implem.Pleasanter.NetCore.dll --urls=http://*:5000 --pathbase=/pleasanter
