// Azure OpenAI and GenAI Resources for Chat UI
// S0 SKU, GPT-4o model in Sweden region

@description('Enable GenAI Chat UI')
param includeChatUI bool = false

@description('Location for main resources')
param location string = 'uksouth'

@description('Name of the Azure OpenAI resource')
param openAIName string = 'expense-openai-${uniqueString(resourceGroup().id)}'

@description('Name of the Azure AI Search resource for RAG')
param searchName string = 'expense-search-${uniqueString(resourceGroup().id)}'

// Azure OpenAI Service (only if includeChatUI is true)
resource openAI 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' = if (includeChatUI) {
  name: openAIName
  location: 'swedencentral' // GPT-4o is available in Sweden
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: openAIName
    publicNetworkAccess: 'Enabled'
  }
}

// Deploy GPT-4o model
resource gpt4oDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-10-01-preview' = if (includeChatUI) {
  parent: openAI
  name: 'gpt-4o'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o'
      version: '2024-08-06'
    }
  }
}

// Azure AI Search for RAG (only if includeChatUI is true)
resource search 'Microsoft.Search/searchServices@2023-11-01' = if (includeChatUI) {
  name: searchName
  location: location
  sku: {
    name: 'basic' // Low cost option
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
  }
}

output openAIEndpoint string = includeChatUI ? openAI.properties.endpoint : ''
output openAIName string = includeChatUI ? openAI.name : ''
output searchEndpoint string = includeChatUI ? 'https://${search.name}.search.windows.net' : ''
output searchName string = includeChatUI ? search.name : ''
output includeChatUI bool = includeChatUI
