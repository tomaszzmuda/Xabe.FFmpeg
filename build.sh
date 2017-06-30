#!/bin/bash
dotnet restore
dotnet test Xabe.FFMpeg.Test/
if [[ -z "${TRAVIS_TAG}" ]]; then 
	exit 0
else
	dotnet pack Xabe.FFMpeg -c Release -o nuget /p:PackageVersion=$TRAVIS_TAG
	dotnet nuget push Xabe.FFMpeg/nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi
