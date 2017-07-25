#!/bin/bash
dotnet restore
dotnet test Xabe.FFMpeg.Test/
if [[ -z "${TRAVIS_TAG}" ]]; then 
	exit 0
else
	dotnet clean Xabe.FFMpeg -c Release
	dotnet build Xabe.FFMPeg -c Release /property:Version=$TRAVIS_TAG
	dotnet pack Xabe.FFMpeg --no-build -c Release -o nuget /p:PackageVersion=$TRAVIS_TAG
	dotnet nuget push Xabe.FFMpeg/nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi
