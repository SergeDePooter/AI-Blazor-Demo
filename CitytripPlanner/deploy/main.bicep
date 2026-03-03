@description('The name of the web app. Must be globally unique.')
param webAppName string = 'citytripplanner-${uniqueString(resourceGroup().id)}'

@description('The location for all resources.')
param location string = resourceGroup().location

@description('The SKU of the App Service Plan.')
@allowed([
  'F1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1v3'
  'P2v3'
  'P3v3'
])
param appServicePlanSku string = 'B1'

@description('The .NET runtime version.')
param netFrameworkVersion string = 'v10.0'

var appServicePlanName = '${webAppName}-plan'
var appInsightsName = '${webAppName}-insights'
var logAnalyticsName = '${webAppName}-logs'

// Log Analytics Workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// Web App
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      alwaysOn: appServicePlanSku != 'F1'
      http20Enabled: true
      healthCheckPath: '/'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ASPNETCORE_HTTPS_PORTS'
          value: '443'
        }
      ]
    }
  }
}

// Output values
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppName string = webApp.name
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output resourceGroupName string = resourceGroup().name
