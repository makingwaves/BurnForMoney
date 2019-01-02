#!/bin/bash
Namespace=BurnForMoney.Functions
FunctionApps=($Namespace $Namespace.InternalApi $Namespace.PublicApi $Namespace.ReadModel $Namespace.Strava)

cd ..
for App in ${FunctionApps[*]}
do
    echo Starting function $App
    cd $App
    func start --build &
    cd ..
done