#!/bin/bash

PREV="5.0.7"

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