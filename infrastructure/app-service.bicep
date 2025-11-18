// App Service Infrastructure for Expense Management System
// Low cost development SKU in UK South region

@description('Name of the App Service Plan')
param appServicePlanName string = 'expense-app-plan'

@description('Name of the Web App')
param webAppName string = 'expense-web-app-${uniqueString(resourceGroup().id)}'

@description('Location for all resources')
param location string = 'uksouth'

@description('The SKU of App Service Plan')
param sku string = 'F1' // Free tier for development

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: sku
    tier: 'Free'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

// Web App
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: false // Not available in Free tier
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
    }
  }
}

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppPrincipalId string = webApp.identity.principalId
