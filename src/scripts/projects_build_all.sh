#!/bin/bash
Namespace=BurnForMoney
FunctionsNamespace=$Namespace.Functions
Projects=($Namespace.Domain $Namespace.Identity $Namespace.Infrastructure $FunctionsNamespace.Shared $FunctionsNamespace $FunctionsNamespace.InternalApi $FunctionsNamespace.PublicApi $FunctionsNamespace.ReadModel $FunctionsNamespace.Strava)

cd ..
for Project in ${Projects[*]}
do
    echo Cleaning and building project $Project
    cd $Project
    dotnet clean & dotnet build &
    cd ..
done