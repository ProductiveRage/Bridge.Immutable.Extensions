@echo off

%~d0
cd "%~p0"

del *.nu*
del *.dll
del *.pdb
del *.xml

copy ..\Bridge.Immutable.Extensions\bin\Release\ProductiveRage.Immutable.Extensions.dll > nul
copy ..\Bridge.Immutable.Extensions\bin\Release\ProductiveRage.Immutable.Extensions.xml > nul

copy ..\ProductiveRage.Immutable.Extensions.nuspec > nul
..\packages\NuGet.CommandLine.3.4.3\tools\nuget pack -NoPackageAnalysis ProductiveRage.Immutable.Extensions.nuspec