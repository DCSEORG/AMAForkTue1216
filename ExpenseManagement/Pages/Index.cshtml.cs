using ExpenseManagement.Models;
using ExpenseManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseManagement.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IExpenseService _expenseService;

    public IndexModel(ILogger<IndexModel> logger, IExpenseService expenseService)
    {
        _logger = logger;
        _expenseService = expenseService;
    }

    public List<Expense> Expenses { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    public async Task OnGetAsync()
    {
        Expenses = await _expenseService.GetExpensesAsync(Filter);
    }
}
