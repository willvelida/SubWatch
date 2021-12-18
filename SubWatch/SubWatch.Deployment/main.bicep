param location string
param cosmosAccountName string
param databaseName string
param containerName string
param functionAppName string
param hostingPlanName string
param storageAccountName string
param appInsightName string
param appConfigName string
param apimInstanceName string
param publisherEmail string
param publisherName string

// Azure Function: App Service Plan
resource subWatchHostingPlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: hostingPlanName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

// Azure Function: Storage Account
resource subWatchStorageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageAccountName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

// Azure Function: App Insights
resource subWatchAppInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Azure Function: Function App
resource subWatchFunctionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  tags: {
    applicationName: 'SubWatch'
  }
  properties: {
    httpsOnly: true
    serverFarmId: subWatchHostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': subWatchAppInsights.properties.InstrumentationKey
        }
        {
          'name': 'AzureWebJobsStorage'
          'value': 'DefaultEndpointsProtocol=https;AccountName=${subWatchStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(subWatchStorageAccount.id, subWatchStorageAccount.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~4'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          'name': 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          'value': 'DefaultEndpointsProtocol=https;AccountName=${subWatchStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(subWatchStorageAccount.id, subWatchStorageAccount.apiVersion).keys[0].value}'
        }
        {
          'name': 'AppConfigurationConnectionString'
          'value': '${listKeys(subWatchAppConfig.id, subWatchAppConfig.apiVersion).value[0].connectionString}'
        }
        {
          'name': 'CosmosDBConnectionString'
          'value': '${listKeys(subWatchCosmosAccount.id, subWatchCosmosAccount.apiVersion).connectionStrings[0].connectionString}'
        }
      ]
    }
  }
  dependsOn: [
    subWatchAppInsights
    subWatchHostingPlan
    subWatchStorageAccount
  ]
}

// APIM
resource subWatchApim 'Microsoft.ApiManagement/service@2021-04-01-preview' = {
  name: apimInstanceName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  sku: {
    capacity: 0
    name: 'Consumption'
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}

// App Configuration
resource subWatchAppConfig 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview' = {
  name: appConfigName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'standard'
  }

  resource databaseConfigPropety 'keyValues' = {
    name: 'Settings:DatabaseName'
    properties: {
      contentType: 'string'
      value: 'SubWatchDB'
    }
  }

  resource containerConfigProperty 'keyValues' = {
    name: 'Settings:ContainerName'
    properties: {
      contentType: 'string'
      value: 'subscriptions'
    } 
  }
}

// Cosmos DB
resource subWatchCosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-07-01-preview' = {
  name: cosmosAccountName
  location: location
  tags: {
    applicationName: 'SubWatch'
  }
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    backupPolicy: {
      type: 'Continuous'
    }
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    createMode: 'Default'
    databaseAccountOfferType: 'Standard'
    enableFreeTier: false
    enableMultipleWriteLocations: false
    locations: [
      {
        failoverPriority: 0
        isZoneRedundant: true
        locationName: location
      }
    ]
    publicNetworkAccess: 'Enabled'
  }
}

resource subWatchDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-07-01-preview' = {
  name: '${subWatchCosmosAccount.name}/${databaseName}'
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource subWatchContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-07-01-preview' = {
  name: '${subWatchDatabase.name}/${containerName}'
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/subscriptionType'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
      }
    }
  }
}
