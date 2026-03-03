# Azure Deployment Guide for CitytripPlanner Web App

This guide explains how to deploy the CitytripPlanner web application to Azure App Service.

## Prerequisites

- Azure subscription
- Azure CLI installed (https://docs.microsoft.com/cli/azure/install-azure-cli)
- GitHub repository with proper permissions to add secrets

## Deployment Options

### Option 1: Deploy using Bicep (Infrastructure as Code)

1. **Login to Azure:**
   ```bash
   az login
   ```

2. **Create a resource group:**
   ```bash
   az group create --name citytripplanner-rg --location westeurope
   ```

3. **Deploy the infrastructure:**
   ```bash
   az deployment group create \
     --resource-group citytripplanner-rg \
     --template-file deploy/main.bicep \
     --parameters deploy/main.parameters.json
   ```

4. **Get the deployment outputs:**
   ```bash
   az deployment group show \
     --resource-group citytripplanner-rg \
     --name main \
     --query properties.outputs
   ```

### Option 2: Deploy using Azure CLI directly

1. **Create an App Service Plan:**
   ```bash
   az appservice plan create \
     --name citytripplanner-plan \
     --resource-group citytripplanner-rg \
     --sku B1 \
     --is-linux
   ```

2. **Create the Web App:**
   ```bash
   az webapp create \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --plan citytripplanner-plan \
     --runtime "DOTNETCORE:10.0"
   ```

3. **Configure the Web App:**
   ```bash
   az webapp config set \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --always-on true \
     --http20-enabled true \
     --min-tls-version 1.2
   ```

## Setting up CI/CD with GitHub Actions

1. **Get the publish profile:**
   ```bash
   az webapp deployment list-publishing-profiles \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --xml
   ```

2. **Add the publish profile as a GitHub secret:**
   - Go to your GitHub repository
   - Navigate to Settings > Secrets and variables > Actions
   - Click "New repository secret"
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: Paste the XML content from step 1

3. **Update the workflow file:**
   - Edit `.github/workflows/azure-deploy.yml`
   - Update `AZURE_WEBAPP_NAME` to match your web app name
   - Commit and push the changes

4. **Trigger the deployment:**
   - Push to the `main` branch, or
   - Manually trigger the workflow from the Actions tab

## Manual Deployment

If you prefer to deploy manually without CI/CD:

1. **Build and publish the application:**
   ```bash
   cd CitytripPlanner
   dotnet publish CitytripPlanner.Web/CitytripPlanner.Web.csproj \
     --configuration Release \
     --output ./publish
   ```

2. **Deploy to Azure:**
   ```bash
   cd publish
   zip -r ../app.zip .
   cd ..
   az webapp deployment source config-zip \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --src app.zip
   ```

## Configuration

### Environment Variables

Set these in the Azure Portal or using Azure CLI:

```bash
az webapp config appsettings set \
  --name citytripplanner-web \
  --resource-group citytripplanner-rg \
  --settings ASPNETCORE_ENVIRONMENT=Production
```

### Custom Domain and SSL

1. **Add custom domain:**
   ```bash
   az webapp config hostname add \
     --webapp-name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --hostname www.yourdomain.com
   ```

2. **Enable HTTPS (managed certificate):**
   ```bash
   az webapp config ssl create \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg \
     --hostname www.yourdomain.com
   ```

## Monitoring

The deployment includes Application Insights for monitoring:

- View logs: Azure Portal > Application Insights > Logs
- Monitor performance: Azure Portal > Application Insights > Performance
- Track failures: Azure Portal > Application Insights > Failures

## Scaling

To scale the application:

```bash
az appservice plan update \
  --name citytripplanner-plan \
  --resource-group citytripplanner-rg \
  --sku P1v3
```

## Troubleshooting

1. **View application logs:**
   ```bash
   az webapp log tail \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg
   ```

2. **Restart the web app:**
   ```bash
   az webapp restart \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg
   ```

3. **Check deployment status:**
   ```bash
   az webapp deployment list \
     --name citytripplanner-web \
     --resource-group citytripplanner-rg
   ```

## Clean Up

To delete all resources:

```bash
az group delete --name citytripplanner-rg --yes --no-wait
```

## Notes

- The default configuration uses the B1 (Basic) pricing tier. Adjust in `deploy/main.parameters.json` as needed.
- For production workloads, consider using Premium (P1v3+) tiers for better performance.
- The web app is configured with HTTPS only and includes security headers.
- Application Insights is included for monitoring and diagnostics.
