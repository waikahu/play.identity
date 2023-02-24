# Play.Identity
Play Economy Identity microservice

## Create and publish package
```powershell
$version="1.0.6"
$owner="waikahu"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```

## Build the Docker image
```powershell
$env:GH_OWNER="waikahu"
$env:GH_PAT="[PAT HERE]"
$crname="wbplayeconomy"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$crname.azurecr.io/play.identity:$version" .
```

## Run the Docker image
```powershell
$adminPass="[PASSWORD HERE]"
$cosmosDbConnString="[Conn HERE]"
$serviceBusConnString="[Conn HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" -e IdentitySettings__AdminUserPassword=$adminPass play.identity:$version
```

## Publishing the Docker image
```powershell
az acr login --name $crname
docker push "$crname.azurecr.io/play.identity:$version"
```

## Create the Kubernetes namespace
```powershell
$namespace="identity"
kubectl create namespace $namespace
```

## Create the Kubernetes secrets
```powershell
kubectl create secret generic identity-secrets --from-literal=cosmodb-connectionstring=$cosmosDbConnString --from-literal=servicebus-connectionstring=$serviceBusConnString --from-literal=admin-password=$adminPass -n $namespace

kubectl get secrets -n $namespace
```

## Create the Kubernetes pod
```powershell
kubectl apply -f .\kubernetes\identity.yaml -n $namespace

# to see list of pods
kubectl get pods -n $namespace
# to see list of services
kubectl get services -n $namespace
# to see the logs of pod
kubectl logs <name of pod> -n $namespace
# to see datailed pod
kubectl describe pod <name of pod> -n $namespace
```