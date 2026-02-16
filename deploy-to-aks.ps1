# Azure Container Registry Tasks - Build and Deploy Script

# Set variables (update these with your values)
$resourceGroup = "aks"
$acrName = "mltestacr6"
$aksCluster = "testaksml"
$imageName = "razorpagesmovie"
$imageTag = "latest"

# Login to Azure (if not already logged in)
# az login

# Create Resource Group (if it doesn't exist)
az group create --name $resourceGroup --location uksouth

# Create ACR (if it doesn't exist)
az acr create --resource-group $resourceGroup --name $acrName --sku Basic

# Build image using ACR Tasks (builds on Azure infrastructure, not your local machine)
# This solves the architecture issue!
az acr build --registry $acrName --image "${imageName}:${imageTag}" --file Dockerfile .

# Alternative: Set up automated builds from GitHub
# az acr task create `
#   --registry $acrName `
#   --name build-razorpagesmovie `
#   --image "${imageName}:${imageTag}" `
#   --context https://github.com/your-username/your-repo.git `
#   --file Dockerfile `
#   --git-access-token YOUR_GITHUB_PAT

# # Get AKS credentials
# az aks get-credentials --resource-group $resourceGroup --name $aksCluster

# # Attach ACR to AKS (allows AKS to pull images from ACR)
# az aks update --resource-group $resourceGroup --name $aksCluster --attach-acr $acrName

# # Update the k8s-deployment.yaml with your ACR name
# (Get-Content k8s-deployment.yaml) -replace '<your-acr-name>', $acrName | Set-Content k8s-deployment.yaml

# # Create secrets in AKS
# kubectl create secret generic razorpagesmovie-secrets `
#   --from-literal=connection-string="Data Source=/app/data/RazorPagesMovie.db" `
#   --dry-run=client -o yaml | kubectl apply -f -

# # Deploy to AKS
# kubectl apply -f k8s-deployment.yaml

# # Get service external IP
# kubectl get service razorpagesmovie-service

# # View logs
# kubectl logs -l app=razorpagesmovie --tail=100

# # Scale deployment
# # kubectl scale deployment razorpagesmovie --replicas=3

# Write-Host "Deployment complete! Wait a few minutes for the LoadBalancer to provision an external IP."
# Write-Host "Run 'kubectl get service razorpagesmovie-service' to check the EXTERNAL-IP"
