#!/bin/bash
Namespace=BurnForMoney.Functions
FunctionApps=($Namespace $Namespace.InternalApi $Namespace.PublicApi $Namespace.ReadModel $Namespace.Strava)

cd ..
for App in ${FunctionApps[*]}
do
    echo Building function $App
    cd $App
    dotnet build
    cd ..
done