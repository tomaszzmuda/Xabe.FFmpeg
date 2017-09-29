#!/bin/bash
dotnet restore
cd Xabe.FFmpeg
dotnet build /p:GenerateDocumentationFile=true
dotnet pack --no-build /p:GenerateDocumentationFile=true
Read-Host -Prompt "Press Enter to continue"