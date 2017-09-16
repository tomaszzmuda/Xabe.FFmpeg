#!/bin/bash
dotnet restore
dotnet test Xabe.FFmpeg.Test/
dotnet clean -c Release
cd Xabe.FFmpeg
if [[ -z "${TRAVIS_TAG}" ]]; then 
	dotnet build /property:Version=1.0.0 /p:GenerateDocumentationFile=true
	dotnet pack --no-build -o nuget /p:PackageVersion=$version /p:GenerateDocumentationFile=true
else
	dotnet build -c Release /property:Version=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet pack --no-build -c Release -o nuget /p:PackageVersion=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet nuget push nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi