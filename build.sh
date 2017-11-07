if [[ -z "${TRAVIS_TAG}" ]]; then 
	dotnet build /p:GenerateDocumentationFile=true
	dotnet pack --no-build -o nuget /p:GenerateDocumentationFile=true
else
	dotnet build -c Release /property:Version=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet pack --no-build -c Release -o nuget /p:PackageVersion=$TRAVIS_TAG /p:GenerateDocumentationFile=true
	dotnet nuget push nuget/*.nupkg -k $NUGET_API -s https://www.nuget.org/api/v2/package
fi