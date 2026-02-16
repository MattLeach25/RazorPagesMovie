#!/bin/bash

# Azure Container Registry Tasks - Build and Deploy Script

# Set variables (update these with your values)
RESOURCE_GROUP="your-resource-group"
ACR_NAME="yourAcrName"
AKS_CLUSTER="your-aks-cluster"
IMAGE_NAME="razorpagesmovie"
IMAGE_TAG="latest"

# Login to Azure (if not already logged in)
# az login

# Create Resource Group (if it doesn't exist)
az group create --name $RESOURCE_GROUP --location eastus

# Create ACR (if it doesn't exist)
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic

# Build image using ACR Tasks (builds on Azure infrastructure, not your local machine)
# This solves the architecture issue!
az acr build --registry $ACR_NAME --image "${IMAGE_NAME}:${IMAGE_TAG}" --file Dockerfile .

# Alternative: Set up automated builds from GitHub
# az acr task create \
#   --registry $ACR_NAME \
#   --name build-razorpagesmovie \
#   --image "${IMAGE_NAME}:${IMAGE_TAG}" \
#   --context https://github.com/your-username/your-repo.git \
#   --file Dockerfile \
#   --git-access-token YOUR_GITHUB_PAT

# Get AKS credentials
az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER

# Attach ACR to AKS (allows AKS to pull images from ACR)
az aks update --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER --attach-acr $ACR_NAME

# Update the k8s-deployment.yaml with your ACR name
sed -i "s/<your-acr-name>/${ACR_NAME}/g" k8s-deployment.yaml

# Create secrets in AKS
kubectl create secret generic razorpagesmovie-secrets \
  --from-literal=connection-string="Data Source=/app/data/RazorPagesMovie.db" \
  --dry-run=client -o yaml | kubectl apply -f -

# Deploy to AKS
kubectl apply -f k8s-deployment.yaml

# Get service external IP
kubectl get service razorpagesmovie-service

# View logs
kubectl logs -l app=razorpagesmovie --tail=100

echo "Deployment complete! Wait a few minutes for the LoadBalancer to provision an external IP."
echo "Run 'kubectl get service razorpagesmovie-service' to check the EXTERNAL-IP"
