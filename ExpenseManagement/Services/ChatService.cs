using Azure;
using Azure.AI.OpenAI;
using ExpenseManagement.Models;
using System.Text.Json;

namespace ExpenseManagement.Services;

public interface IChatService
{
    Task<string> ProcessMessageAsync(string message);
}

public class ChatService : IChatService
{
    private readonly IConfiguration _configuration;
    private readonly IExpenseService _expenseService;
    private readonly OpenAIClient? _openAIClient;
    private readonly string? _deploymentName;
    private readonly bool _isEnabled;

    public ChatService(IConfiguration configuration, IExpenseService expenseService)
    {
        _configuration = configuration;
        _expenseService = expenseService;
        
        // Check if Chat UI is enabled
        _isEnabled = configuration.GetValue<bool>("IncludeChatUI");
        
        if (_isEnabled)
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var apiKey = configuration["AzureOpenAI:ApiKey"];
            _deploymentName = configuration["AzureOpenAI:DeploymentName"];

            if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
            {
                _openAIClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            }
        }
    }

    public async Task<string> ProcessMessageAsync(string message)
    {
        if (!_isEnabled || _openAIClient == null || string.IsNullOrEmpty(_deploymentName))
        {
            return "Chat UI is not enabled. Please deploy with --with-chat-ui flag to enable this feature.";
        }

        try
        {
            // Define available functions for the AI to call
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                Messages =
                {
                    new ChatRequestSystemMessage(@"You are a helpful AI assistant for an expense management system. 
You can help users view, create, and manage their expenses.

Available functions:
- get_expenses: Retrieve all expenses with optional filter
- get_pending_expenses: Get expenses waiting for approval
- create_expense: Create a new expense
- submit_expense: Submit an expense for approval
- approve_expense: Approve or reject an expense

Always be helpful and provide clear responses about expense data."),
                    new ChatRequestUserMessage(message)
                },
                Temperature = 0.7f,
                MaxTokens = 800,
                Functions =
                {
                    new FunctionDefinition
                    {
                        Name = "get_expenses",
                        Description = "Get all expenses with optional filter",
                        Parameters = BinaryData.FromObjectAsJson(new
                        {
                            type = "object",
                            properties = new
                            {
                                filter = new { type = "string", description = "Optional filter for category, description, or status" }
                            }
                        })
                    },
                    new FunctionDefinition
                    {
                        Name = "get_pending_expenses",
                        Description = "Get pending expenses that need approval",
                        Parameters = BinaryData.FromObjectAsJson(new
                        {
                            type = "object",
                            properties = new { }
                        })
                    },
                    new FunctionDefinition
                    {
                        Name = "create_expense",
                        Description = "Create a new expense",
                        Parameters = BinaryData.FromObjectAsJson(new
                        {
                            type = "object",
                            properties = new
                            {
                                amount = new { type = "number", description = "Expense amount in GBP" },
                                category = new { type = "string", description = "Category like Travel, Meals, Supplies" },
                                description = new { type = "string", description = "Description of the expense" },
                                date = new { type = "string", description = "Expense date in YYYY-MM-DD format" }
                            },
                            required = new[] { "amount", "category" }
                        })
                    }
                }
            };

            var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
            var choice = response.Value.Choices[0];

            // Check if AI wants to call a function
            if (choice.FinishReason == CompletionsFinishReason.FunctionCall)
            {
                var functionCall = choice.Message.FunctionCall;
                var functionResult = await ExecuteFunctionAsync(functionCall.Name, functionCall.Arguments);

                // Send function result back to AI for natural language response
                chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(choice.Message));
                chatCompletionsOptions.Messages.Add(new ChatRequestFunctionMessage(functionCall.Name, functionResult));

                var secondResponse = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                return secondResponse.Value.Choices[0].Message.Content;
            }

            return choice.Message.Content;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private async Task<string> ExecuteFunctionAsync(string functionName, string arguments)
    {
        try
        {
            switch (functionName)
            {
                case "get_expenses":
                    var getArgs = JsonSerializer.Deserialize<Dictionary<string, string>>(arguments);
                    var filter = getArgs?.GetValueOrDefault("filter");
                    var expenses = await _expenseService.GetExpensesAsync(filter);
                    return JsonSerializer.Serialize(expenses);

                case "get_pending_expenses":
                    var pending = await _expenseService.GetPendingExpensesAsync();
                    return JsonSerializer.Serialize(pending);

                case "create_expense":
                    var createArgs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(arguments);
                    if (createArgs == null) return "Invalid arguments";

                    var amount = createArgs["amount"].GetDecimal();
                    var category = createArgs["category"].GetString() ?? "";
                    var description = createArgs.GetValueOrDefault("description").GetString();
                    var dateStr = createArgs.GetValueOrDefault("date").GetString();
                    var expenseDate = string.IsNullOrEmpty(dateStr) ? DateTime.Today : DateTime.Parse(dateStr);

                    // Find category ID
                    var categories = await _expenseService.GetCategoriesAsync();
                    var categoryObj = categories.FirstOrDefault(c => 
                        c.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
                    
                    if (categoryObj == null)
                    {
                        categoryObj = categories.First(); // Default to first category
                    }

                    var request = new CreateExpenseRequest
                    {
                        UserId = 1,
                        CategoryId = categoryObj.CategoryId,
                        Amount = amount,
                        ExpenseDate = expenseDate,
                        Description = description
                    };

                    var expense = await _expenseService.CreateExpenseAsync(request);
                    await _expenseService.SubmitExpenseAsync(expense.ExpenseId);
                    
                    return JsonSerializer.Serialize(new { success = true, expense });

                default:
                    return "Function not found";
            }
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message });
        }
    }
}
