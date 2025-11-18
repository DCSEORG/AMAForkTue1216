// Main deployment orchestration
// Deploys all Azure resources for the Expense Management System

@description('Enable GenAI Chat UI (default: false)')
param includeChatUI bool = false

@description('Location for all resources')
param location string = 'uksouth'

@description('Resource group name')
param resourceGroupName string = 'expense-management-rg'

// Deploy App Service
module appService 'app-service.bicep' = {
  name: 'appServiceDeployment'
  params: {
    location: location
  }
}

// Deploy GenAI resources (conditional)
module genAI 'genai-resources.bicep' = {
  name: 'genAIDeployment'
  params: {
    includeChatUI: includeChatUI
    location: location
  }
}

// Outputs
output webAppName string = appService.outputs.webAppName
output webAppUrl string = appService.outputs.webAppUrl
output webAppPrincipalId string = appService.outputs.webAppPrincipalId
output openAIEndpoint string = genAI.outputs.openAIEndpoint
output openAIName string = genAI.outputs.openAIName
output searchEndpoint string = genAI.outputs.searchEndpoint
output chatUIEnabled bool = genAI.outputs.includeChatUI
