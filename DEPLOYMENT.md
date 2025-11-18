# Deployment Guide - Expense Management System

This guide will walk you through deploying the modernized Expense Management System to Azure.

## Prerequisites

Before you begin, ensure you have:

1. **Azure CLI** installed and updated
   ```bash
   az --version
   ```
   If not installed, visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

2. **Active Azure Subscription**
   - You'll need contributor or owner access
   - Free tier or pay-as-you-go subscription works

3. **Git** installed (to clone the repository)

## Step-by-Step Deployment

### Step 1: Login to Azure

```bash
az login
```

This will open a browser window for authentication. Follow the prompts to sign in.

### Step 2: Set Your Subscription (if you have multiple)

```bash
# List available subscriptions
az account list --output table

# Set the desired subscription
az account set --subscription "Your Subscription Name or ID"

# Verify the active subscription
az account show --query name
```

### Step 3: Clone the Repository (if not already done)

```bash
git clone <your-repo-url>
cd AMAForkTue1216
```

### Step 4: Choose Your Deployment Option

#### Option A: Basic Deployment (Recommended for First-Time Users)

This deploys the core expense management application without AI features:

```bash
chmod +x deploy.sh
./deploy.sh
```

**What gets deployed:**
- App Service Plan (Free tier)
- Web App with Managed Identity
- Expense Management Application

**Estimated cost:** $0/month (Free tier)

#### Option B: Full Deployment with GenAI Chat

This deploys everything including AI-powered chat:

```bash
chmod +x deploy.sh
./deploy.sh --with-chat-ui
```

**What gets deployed:**
- Everything from Option A, plus:
- Azure OpenAI Service (S0 SKU) in Sweden Central
- Azure AI Search (Basic tier)
- AI Chat Interface

**Estimated cost:** ~$30-50/month (depending on usage)

### Step 5: Wait for Deployment

The deployment script will:
1. Create a resource group: `expense-management-rg`
2. Deploy Azure infrastructure (3-5 minutes)
3. Configure application settings
4. Deploy the application code

You'll see progress messages throughout the process.

### Step 6: Access Your Application

When deployment completes, you'll see:

```
=== Deployment Complete ===
Web App URL: https://expense-web-app-abc123.azurewebsites.net/Index
```

**Important:** Make sure to navigate to `/Index` not just the root URL!

### Available Endpoints

- **Expenses List:** `https://<your-app>.azurewebsites.net/Index`
- **Add Expense:** `https://<your-app>.azurewebsites.net/AddExpense`
- **Approve Expenses:** `https://<your-app>.azurewebsites.net/ApproveExpenses`
- **API Docs (Swagger):** `https://<your-app>.azurewebsites.net/swagger`
- **AI Chat** (if enabled): `https://<your-app>.azurewebsites.net/Chat`

## Testing Your Deployment

### 1. View Expenses
- Navigate to the Index page
- You should see sample expense data
- Try using the filter to search for specific expenses

### 2. Add an Expense
- Click "Add Expense" in the navigation
- Fill in the form:
  - Amount: 50.00
  - Date: Today's date
  - Category: Travel
  - Description: Test expense
- Click Submit
- You should be redirected to the Expenses page with your new expense

### 3. Approve Expenses
- Navigate to "Approve Expenses"
- You'll see pending expenses
- Click "Approve" or "Reject" on any expense
- The expense status will update immediately

### 4. Test the API
- Navigate to `/swagger`
- Explore the available API endpoints
- Try the "Execute" button to test APIs directly

### 5. Test AI Chat (if enabled)
- Navigate to "AI Chat"
- Try asking:
  - "Show me all pending expenses"
  - "Create a new expense for £25 for meals"
  - "What's the total of all expenses?"

## Troubleshooting

### Issue: Application won't load

**Solution:** 
- Make sure you're accessing `/Index` not just the root URL
- Wait 1-2 minutes after deployment for the app to fully start
- Check the Azure Portal for any deployment errors

### Issue: Chat UI says "not enabled"

**Cause:** You deployed without the `--with-chat-ui` flag

**Solution:** Re-run the deployment:
```bash
./deploy.sh --with-chat-ui
```

### Issue: Deployment fails

**Common causes:**
- Not logged into Azure CLI: Run `az login`
- Insufficient permissions: Ensure you have Contributor role
- Region not available: Try changing LOCATION in deploy.sh
- Resource naming conflict: Azure resources need unique names

**Solution:**
```bash
# Check login status
az account show

# Try deleting and recreating
az group delete --name expense-management-rg --yes
./deploy.sh
```

### Issue: Can't access Swagger docs

**Cause:** Swagger is enabled for all environments in this demo

**Solution:** 
- Navigate to `https://<your-app>.azurewebsites.net/swagger`
- If still not working, check App Service logs in Azure Portal

## Customizing Your Deployment

### Change Resource Group Name

Edit `deploy.sh`:
```bash
RESOURCE_GROUP="my-custom-rg-name"
```

### Change Azure Region

Edit `deploy.sh`:
```bash
LOCATION="eastus"  # or "westeurope", "australiaeast", etc.
```

### Change App Service SKU

Edit `infrastructure/app-service.bicep`:
```bicep
param sku string = 'B1'  // Change from 'F1' to 'B1' for basic tier
```

## Monitoring and Logs

### View Application Logs

```bash
az webapp log tail \
  --resource-group expense-management-rg \
  --name <your-web-app-name>
```

### View in Azure Portal

1. Go to https://portal.azure.com
2. Navigate to Resource Groups > expense-management-rg
3. Click on your App Service
4. In the left menu:
   - **Log stream:** See real-time logs
   - **Metrics:** View performance data
   - **Diagnose and solve problems:** Troubleshooting tools

## Cost Management

### Check Current Costs

```bash
az consumption usage list \
  --resource-group expense-management-rg \
  --output table
```

### Estimated Monthly Costs

**Basic Deployment (no AI):**
- App Service (F1 Free tier): $0
- **Total: $0/month**

**With GenAI Chat:**
- App Service (F1 Free tier): $0
- Azure OpenAI (S0): ~$30-40/month
- Azure AI Search (Basic): ~$10/month
- **Total: ~$40-50/month**

## Cleaning Up

To delete all resources and stop incurring charges:

```bash
az group delete --name expense-management-rg --yes --no-wait
```

This will delete:
- App Service and App Service Plan
- Azure OpenAI (if deployed)
- Azure AI Search (if deployed)
- All other resources in the group

## Next Steps

### For Production Use

This is a **proof-of-concept** deployment. For production:

1. **Add Authentication:**
   - Implement Azure AD authentication
   - Add role-based access control (RBAC)

2. **Connect to Database:**
   - Deploy Azure SQL Database
   - Update connection strings
   - Use Managed Identity for authentication

3. **Secure Secrets:**
   - Use Azure Key Vault for API keys
   - Remove secrets from configuration files

4. **Scale Appropriately:**
   - Move to B1 or higher App Service Plan
   - Enable auto-scaling
   - Consider Azure Front Door for CDN

5. **Add Monitoring:**
   - Enable Application Insights
   - Set up alerts for errors and performance
   - Configure log analytics

6. **Implement CI/CD:**
   - Set up GitHub Actions or Azure DevOps
   - Automate testing and deployment
   - Use staging slots

## Support

For issues or questions:
1. Check the [README.md](README.md)
2. Review the [ARCHITECTURE.md](ARCHITECTURE.md)
3. Check Azure documentation: https://docs.microsoft.com/azure
4. Review deployment logs and Azure Portal diagnostics

## Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure OpenAI Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/openai/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bicep Language Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [Azure CLI Reference](https://docs.microsoft.com/en-us/cli/azure/)

---

**Happy Deploying! 🚀**
