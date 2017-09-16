#!/bin/bash
dotnet restore
dotnet test Xabe.FFmpeg.Test/
dotnet clean -c Release
cd Xabe.FFmpeg
if [[ -z "${TRAVIS_TAG}" ]]; then 
	version=1.0.0
else
	version=$TRAVIS_TAG
fi

dotnet build -c Release /property:Version=$version /p:GenerateDocumentationFile=true
dotnet pack --no-build -c Release -o nuget /p:PackageVersion=$version /p:GenerateDocumentationFile=true

if [[ -z "${$NUGET_API}" ]]; then 
	dotnet nuget push nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi