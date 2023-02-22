# Play.Identity
Play Economy Identity microservice

## Create and publish package
```powershell
$version="1.0.2"
$owner="waikahu"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```

## Build the Docker image
```powershell
$env:GH_OWNER="waikahu"
$env:GH_PAT="[PAT HERE]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.identity:$version .
```