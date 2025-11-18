# Azure Services Architecture

This diagram shows the Azure services deployed by this solution and how they connect to each other.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Azure Subscription                          │
│                                                                     │
│  ┌───────────────────────────────────────────────────────────────┐ │
│  │              Resource Group: expense-management-rg           │ │
│  │                                                               │ │
│  │  ┌─────────────────────┐                                     │ │
│  │  │   App Service Plan   │                                     │ │
│  │  │    (F1 - Free)      │                                     │ │
│  │  │   Linux + .NET 8.0   │                                     │ │
│  │  └──────────┬───────────┘                                     │ │
│  │             │                                                 │ │
│  │             ▼                                                 │ │
│  │  ┌─────────────────────┐                                     │ │
│  │  │    Web App          │                                     │ │
│  │  │  (Expense System)   │                                     │ │
│  │  │                     │                                     │ │
│  │  │  - ASP.NET Razor    │                                     │ │
│  │  │  - REST APIs        │                                     │ │
│  │  │  - Swagger UI       │                                     │ │
│  │  │  - Dummy Data       │                                     │ │
│  │  │                     │                                     │ │
│  │  │ ┌─────────────────┐ │                                     │ │
│  │  │ │ Managed Identity │ │                                     │ │
│  │  │ │   (Optional)     │ │                                     │ │
│  │  │ └─────────────────┘ │                                     │ │
│  │  └──────────┬───────────┘                                     │ │
│  │             │                                                 │ │
│  │             │ (Optional - if includeChatUI=true)             │ │
│  │             ▼                                                 │ │
│  │  ┌─────────────────────────────────────────┐                 │ │
│  │  │                                         │                 │ │
│  │  │  ┌────────────────────────────────┐    │                 │ │
│  │  │  │  Azure OpenAI Service          │    │                 │ │
│  │  │  │  (S0 SKU - Sweden Central)     │    │                 │ │
│  │  │  │                                 │    │                 │ │
│  │  │  │  Model: GPT-4o                  │    │                 │ │
│  │  │  │  - Chat completion              │    │                 │ │
│  │  │  │  - Function calling             │    │                 │ │
│  │  │  │  - RAG integration              │    │                 │ │
│  │  │  └────────────────────────────────┘    │                 │ │
│  │  │                                         │                 │ │
│  │  │  ┌────────────────────────────────┐    │                 │ │
│  │  │  │  Azure AI Search               │    │                 │ │
│  │  │  │  (Basic SKU - UK South)        │    │                 │ │
│  │  │  │                                 │    │                 │ │
│  │  │  │  - RAG document storage         │    │                 │ │
│  │  │  │  - Semantic search              │    │                 │ │
│  │  │  └────────────────────────────────┘    │                 │ │
│  │  │                                         │                 │ │
│  │  └─────────────────────────────────────────┘                 │ │
│  │                                                               │ │
│  └───────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘

                              ┌────────────┐
                              │   Users    │
                              │  (Browser) │
                              └──────┬─────┘
                                     │
                    ┌────────────────┼────────────────┐
                    │                │                │
                    ▼                ▼                ▼
            ┌──────────────┐  ┌──────────┐  ┌──────────────┐
            │  /Index      │  │ /Swagger │  │  /Chat       │
            │  (Expenses)  │  │ (API)    │  │  (GenAI)     │
            └──────────────┘  └──────────┘  └──────────────┘
```

## Components

### Core Infrastructure (Always Deployed)

1. **App Service Plan (F1 - Free)**
   - Linux-based hosting
   - .NET 8.0 runtime
   - UK South region
   - Low-cost development SKU

2. **Web App**
   - ASP.NET Core Razor Pages application
   - REST API endpoints with Swagger documentation
   - System-assigned Managed Identity (for future database access)
   - HTTPS only
   - Dummy data service (no database required)

### Optional GenAI Components (Deployed when includeChatUI=true)

3. **Azure OpenAI Service**
   - Location: Sweden Central (GPT-4o model availability)
   - SKU: S0 (Standard)
   - Model: GPT-4o (deployment name: "gpt-4o")
   - Capabilities: Chat completion, function calling
   - Used for natural language interaction with expense data

4. **Azure AI Search**
   - Location: UK South
   - SKU: Basic (low-cost development)
   - Purpose: Retrieval-Augmented Generation (RAG) pattern
   - Stores contextual information for AI chat

## Connectivity

- **Public Internet** → **Web App** (HTTPS)
- **Web App** → **Azure OpenAI** (REST API, when Chat UI enabled)
- **Web App** → **Azure AI Search** (REST API, when Chat UI enabled)
- **Managed Identity** → Can be configured for Azure SQL or other services (future use)

## Deployment Modes

### Basic Deployment (Default)
```bash
./deploy.sh
```
Deploys: App Service + Web App only
Cost: Minimal (Free tier for App Service)

### Full Deployment with Chat UI
```bash
./deploy.sh --with-chat-ui
```
Deploys: App Service + Web App + Azure OpenAI + Azure AI Search
Cost: Low (S0/Basic tiers for AI services)

## Security Features

- HTTPS enforced on all endpoints
- Managed Identity enabled for secure Azure service-to-service authentication
- API keys stored in App Settings (production should use Azure Key Vault)
- TLS 1.2 minimum
- FTPS disabled

## Access Points

- **Main Application**: `https://<app-name>.azurewebsites.net/Index`
- **API Documentation**: `https://<app-name>.azurewebsites.net/swagger`
- **Chat UI** (if enabled): `https://<app-name>.azurewebsites.net/Chat`
