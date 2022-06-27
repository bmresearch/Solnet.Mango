#!/bin/bash

PREV="6.0.13"

FILES="README.md
SharedBuildProperties.props
Solnet.Mango/Solnet.Mango.csproj
Solnet.Mango.Examples/Solnet.Mango.Examples.csproj
Solnet.Mango.Test/Solnet.Mango.Test.csproj
chver.sh"

for f in $FILES
do
    echo $f
    sed -i "s/$PREV/$1/g" $f
done