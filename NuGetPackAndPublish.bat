cd C:\Users\junwin\Source\Workspaces\K3Base\K3Channel
del *.nupkg
nuget pack  K3Channel.csproj -IncludeReferencedProjects
NuGet Push K3Channel*.nupkg
