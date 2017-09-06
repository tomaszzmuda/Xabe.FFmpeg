#!/bin/bash
dotnet restore
dotnet test Xabe.FFmpeg.Test/
if [[ -z "${TRAVIS_TAG}" ]]; then 
	exit 0
else
	cd Xabe.FFmpeg
	dotnet clean -c Release
	dotnet build -c Release /property:Version=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet pack --no-build -c Release -o nuget /p:PackageVersion=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet nuget push nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi
