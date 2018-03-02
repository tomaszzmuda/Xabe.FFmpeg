$dotnetPath = get-command dotnet | Select-Object -ExpandProperty Definition
OpenCover.Console.exe -target:"$dotnetPath" -targetargs:"test Xabe.FFmpeg.Test\Xabe.FFmpeg.Test.csproj" -mergeoutput -hideskipped:File -output:coverage.xml -oldStyle -searchdirs:"Xabe.FFmpeg.Test\bin\Debug\netcoreapp2.0" -filter:"+[*]* -[Xabe.FFmpeg.Test]*" -register:user 
ReportGenerator.exe -reports:coverage.xml -targetdir:coverage -verbosity:Error	
Read-Host "Press ENTER"