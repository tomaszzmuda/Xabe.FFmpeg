#!/bin/bash
dotnet restore
cd Xabe.FFmpeg
dotnet build /property:Version=1.0.0 /p:GenerateDocumentationFile=true
dotnet pack --no-build -o ../ /p:PackageVersion=$version /p:GenerateDocumentationFile=true
