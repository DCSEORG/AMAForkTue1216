#!/bin/bash
# Deployment script for Expense Management System
# This script deploys all Azure infrastructure and the application

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== Expense Management System Deployment ===${NC}"

# Configuration
RESOURCE_GROUP="expense-management-rg"
LOCATION="uksouth"
INCLUDE_CHAT_UI="false"  # Set to "true" to enable GenAI chat UI

# Check if user wants to enable Chat UI
if [ "$1" == "--with-chat-ui" ]; then
    INCLUDE_CHAT_UI="true"
    echo -e "${YELLOW}Chat UI will be deployed${NC}"
else
    echo -e "${YELLOW}Chat UI will NOT be deployed (use --with-chat-ui to enable)${NC}"
fi

# Check Azure CLI is logged in
echo -e "\n${GREEN}Step 1: Checking Azure CLI login...${NC}"
if ! az account show &> /dev/null; then
    echo -e "${RED}Error: Not logged in to Azure CLI${NC}"
    echo "Please run 'az login' first"
    exit 1
fi

SUBSCRIPTION_NAME=$(az account show --query name -o tsv)
echo -e "Logged in to subscription: ${GREEN}${SUBSCRIPTION_NAME}${NC}"

# Create resource group
echo -e "\n${GREEN}Step 2: Creating resource group...${NC}"
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output none

echo -e "${GREEN}Resource group created: ${RESOURCE_GROUP}${NC}"

# Deploy infrastructure
echo -e "\n${GREEN}Step 3: Deploying Azure infrastructure...${NC}"
DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group $RESOURCE_GROUP \
    --template-file infrastructure/main.bicep \
    --parameters includeChatUI=$INCLUDE_CHAT_UI location=$LOCATION \
    --output json)

# Extract outputs
WEB_APP_NAME=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.webAppName.value')
WEB_APP_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.webAppUrl.value')
OPENAI_ENDPOINT=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.openAIEndpoint.value')
OPENAI_NAME=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.openAIName.value')

echo -e "${GREEN}Infrastructure deployed successfully!${NC}"
echo -e "Web App Name: ${GREEN}${WEB_APP_NAME}${NC}"

# Configure App Settings
echo -e "\n${GREEN}Step 4: Configuring application settings...${NC}"

if [ "$INCLUDE_CHAT_UI" == "true" ]; then
    # Get OpenAI key
    OPENAI_KEY=$(az cognitiveservices account keys list \
        --name $OPENAI_NAME \
        --resource-group $RESOURCE_GROUP \
        --query key1 -o tsv)
    
    az webapp config appsettings set \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --settings \
            "IncludeChatUI=true" \
            "AzureOpenAI__Endpoint=$OPENAI_ENDPOINT" \
            "AzureOpenAI__DeploymentName=gpt-4o" \
        --output none
    
    # Store key as secret (in production, use Key Vault)
    az webapp config appsettings set \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --settings "AzureOpenAI__ApiKey=$OPENAI_KEY" \
        --output none
else
    az webapp config appsettings set \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --settings "IncludeChatUI=false" \
        --output none
fi

echo -e "${GREEN}Application settings configured${NC}"

# Deploy application code
echo -e "\n${GREEN}Step 5: Deploying application code...${NC}"

if [ -f "app.zip" ]; then
    az webapp deploy \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --src-path ./app.zip \
        --type zip \
        --output none
    
    echo -e "${GREEN}Application deployed successfully!${NC}"
else
    echo -e "${YELLOW}Warning: app.zip not found. Skipping application deployment.${NC}"
    echo -e "${YELLOW}Build and zip the application first, then run:${NC}"
    echo -e "az webapp deploy --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --src-path ./app.zip"
fi

# Display results
echo -e "\n${GREEN}=== Deployment Complete ===${NC}"
echo -e "Web App URL: ${GREEN}${WEB_APP_URL}/Index${NC}"
echo -e "${YELLOW}Important: Navigate to ${WEB_APP_URL}/Index (not just the root URL)${NC}"
echo ""
echo -e "Resource Group: ${GREEN}${RESOURCE_GROUP}${NC}"
echo -e "Location: ${GREEN}${LOCATION}${NC}"
echo -e "Chat UI Enabled: ${GREEN}${INCLUDE_CHAT_UI}${NC}"
echo ""
echo -e "${GREEN}Next steps:${NC}"
echo "1. Navigate to the Web App URL above to view your application"
echo "2. Check the Azure Portal to verify all resources are running"
if [ "$INCLUDE_CHAT_UI" == "true" ]; then
    echo "3. Access the chat UI at ${WEB_APP_URL}/Chat"
fi
echo ""
