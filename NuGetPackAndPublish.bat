cd C:\Users\junwi\source\sandpit\K3Channel
del *.nupkg
.nuget\nuget pack  K3Channel.csproj -IncludeReferencedProjects
.nuget\NuGet Push K3Channel*.nupkg
