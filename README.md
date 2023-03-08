# Play.Identity
Play Economy Identity microservice

## Create and publish package
```powershell
$version="1.1.1"
$owner="waikahu"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```

## Build the Docker image
```powershell
$env:GH_OWNER="waikahu"
$env:GH_PAT="[PAT HERE]"
$appname="wbplayeconomy"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$appname.azurecr.io/play.identity:$version" .
```

## Run the Docker image
```powershell
$adminPass="[PASSWORD HERE]"
$cosmosDbConnString="[Conn HERE]"
$serviceBusConnString="[Conn HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" -e ServiceSettings__KeyVaultName="wbplayeconomy" -e IdentitySettings__AdminUserPassword=$adminPass "$appname.azurecr.io/play.identity:$version"
```

## Publishing the Docker image
```powershell
az acr login --name $appname
docker push "$appname.azurecr.io/play.identity:$version"
```

## Create the Kubernetes namespace
```powershell
$namespace="identity"
kubectl create namespace $namespace
```

## Create the Kubernetes pod
```powershell
kubectl apply -f .\kubernetes\identity.yaml -n $namespace

# to see list of pods
kubectl get pods -n $namespace -w
# to see list of services
kubectl get services -n $namespace
# to see the logs of pod
kubectl logs <name of pod> -n $namespace
# to see datailed pod
kubectl describe pod <name of pod> -n $namespace

kubectl rollout restart deployment $namespace-deployment -n $namespace
```

## Creating the Azure Managed Identity and granting it access to the Key Vault secrets
```powershell
$appname="wbplayeconomy"
az identity create --resource-group $appname --name $namespace
$IDENTITY_CLIENT_ID=az identity show -g $appname -n $namespace --query clientId -otsv
# i've to put the appid manually in the Azure Key Vault # 
az keyvault set-policy -n $appname --secret-permissions get list --spn $IDENTITY_CLIENT_ID
```

## Establish the federated identity credential
```powershell
$AKS_OIDC_ISSUER=az aks show -n $appname -g $appname --query "oidcIssuerProfile.issuerUrl" -otsv

az identity federated-credential create --name $namespace --identity-name $namespace --resource-group $appname --issuer $AKS_OIDC_ISSUER --subject "system:serviceaccount:${namespace}:${namespace}-serviceaccount"
```

## Creating the signing certificate
```powershell
kubectl apply -f .\kubernetes\signing-cert.yaml -n $namespace
```