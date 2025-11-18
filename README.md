![Header image](https://github.com/DougChisholm/App-Mod-Assist/blob/main/repo-header.png)

# Expense Management System - Modernized

A cloud-native Azure expense management application built from legacy screenshots and database schema. This project demonstrates how GitHub Copilot can modernize legacy applications into modern cloud-native solutions.

## 🚀 Features

- **Modern Web Interface**: Clean, responsive ASP.NET Core Razor Pages UI
- **RESTful APIs**: Complete API endpoints with Swagger documentation
- **Expense Tracking**: Submit, view, and filter expenses
- **Approval Workflow**: Manager approval interface for pending expenses
- **GenAI Chat** (Optional): Natural language interaction with expense data using Azure OpenAI
- **Cloud-Native**: Fully deployable to Azure with Infrastructure as Code (Bicep)

## 📋 Prerequisites

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed
- An active Azure subscription
- Basic familiarity with command line operations

## 🏗️ Quick Start

### Option 1: Basic Deployment (Recommended for getting started)

Deploy the expense management system **without** GenAI chat features:

```bash
# 1. Clone the repository
git clone <repository-url>
cd AMAForkTue1216

# 2. Login to Azure
az login

# 3. Set your subscription (optional, if you have multiple subscriptions)
az account set --subscription "<your-subscription-name-or-id>"

# 4. Deploy the infrastructure and application
chmod +x deploy.sh
./deploy.sh
```

This deploys:
- App Service (Free tier)
- Expense Management Web Application
- REST APIs with Swagger documentation

**Cost**: Minimal (uses Free tier)

### Option 2: Full Deployment with GenAI Chat UI

Deploy the complete system including AI-powered chat interface:

```bash
./deploy.sh --with-chat-ui
```

This deploys everything from Option 1, plus:
- Azure OpenAI Service (S0 SKU, GPT-4o model)
- Azure AI Search (Basic tier for RAG)
- Chat UI for natural language expense management

**Cost**: Low (S0 and Basic tiers for AI services)

## 🌐 Accessing Your Application

After deployment completes, you'll see output similar to:

```
=== Deployment Complete ===
Web App URL: https://expense-web-app-abc123.azurewebsites.net/Index
```

### Important URLs

- **Main Application**: `https://<your-app-name>.azurewebsites.net/Index`
- **Add Expense**: `https://<your-app-name>.azurewebsites.net/AddExpense`
- **Approve Expenses**: `https://<your-app-name>.azurewebsites.net/ApproveExpenses`
- **API Documentation**: `https://<your-app-name>.azurewebsites.net/swagger`
- **Chat UI** (if enabled): `https://<your-app-name>.azurewebsites.net/Chat`

⚠️ **Note**: Always navigate to `/Index` - the root URL may not display the application correctly.

## 📊 Application Features

### Expense List Page
- View all expenses with filtering capability
- Filter by category, description, or status
- Color-coded status badges (Draft, Submitted, Approved, Rejected)

### Add Expense Page
- Submit new expenses with amount, date, category, and description
- Dropdown selection for expense categories (Travel, Meals, Supplies, etc.)
- Automatic submission for approval

### Approve Expenses Page
- Manager view of pending (submitted) expenses
- Approve or reject expenses with one click
- Filter pending expenses

### REST API
- `GET /api/expenses` - List all expenses
- `GET /api/expenses/pending` - List pending expenses
- `POST /api/expenses` - Create new expense
- `POST /api/expenses/{id}/submit` - Submit for approval
- `POST /api/expenses/approve` - Approve/reject expense
- `GET /api/categories` - List expense categories
- Full Swagger documentation at `/swagger`

## 🏗️ Architecture

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed architecture diagram and component descriptions.

**Key Components**:
- ASP.NET Core 8.0 Razor Pages application
- Dummy data service (no database required for POC)
- Managed Identity enabled for future database connectivity
- Optional Azure OpenAI GPT-4o integration
- Optional Azure AI Search for RAG pattern

## 🔧 Development

### Local Development

```bash
cd ExpenseManagement
dotnet run
```

Access locally at: `https://localhost:5001/Index`

### Building the Deployment Package

```bash
cd ExpenseManagement
dotnet publish -c Release -o ../publish
cd ../publish
zip -r ../app.zip .
```

## 📁 Project Structure

```
├── infrastructure/           # Azure Bicep IaC files
│   ├── main.bicep           # Main orchestration
│   ├── app-service.bicep    # App Service resources
│   └── genai-resources.bicep # AI services (optional)
├── ExpenseManagement/        # ASP.NET Core application
│   ├── Pages/               # Razor Pages (UI)
│   ├── Controllers/         # API Controllers
│   ├── Models/              # Data models
│   ├── Services/            # Business logic
│   └── wwwroot/             # Static files
├── deploy.sh                 # Deployment script
├── app.zip                   # Pre-built deployment package
└── ARCHITECTURE.md           # Architecture documentation
```

## 🔒 Security Considerations

This is a **proof-of-concept** deployment for demonstration purposes:

- Uses dummy data (no real database)
- API keys stored in App Settings (production should use Azure Key Vault)
- No authentication/authorization implemented
- Free/Basic tiers used for cost optimization

**For Production Use**:
- Implement Azure AD authentication
- Use Azure Key Vault for secrets
- Connect to Azure SQL Database with Managed Identity
- Scale to appropriate SKUs
- Implement proper logging and monitoring
- Add rate limiting and DDoS protection

## 📝 Customization

### Change Resource Group or Location

Edit `deploy.sh`:
```bash
RESOURCE_GROUP="your-resource-group-name"
LOCATION="your-location"  # e.g., "eastus", "westeurope"
```

### Enable/Disable Chat UI

Default is disabled. Enable by:
```bash
./deploy.sh --with-chat-ui
```

Or edit `deploy.sh` to change the default:
```bash
INCLUDE_CHAT_UI="true"
```

## 🧹 Clean Up

To delete all deployed resources:

```bash
az group delete --name expense-management-rg --yes --no-wait
```

## 📚 Additional Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure OpenAI Service Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/openai/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)

## 🤝 Contributing

This project was generated by GitHub Copilot as a demonstration of app modernization capabilities.

## 📄 License

See [LICENSE](LICENSE) file for details.

---

**Generated by GitHub Copilot** | **Modernized from Legacy Screenshots** | **Powered by Azure**
