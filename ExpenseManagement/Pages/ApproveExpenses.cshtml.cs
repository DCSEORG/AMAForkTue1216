using ExpenseManagement.Models;
using ExpenseManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseManagement.Pages;

public class ApproveExpensesModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public ApproveExpensesModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public List<Expense> PendingExpenses { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        PendingExpenses = await _expenseService.GetPendingExpensesAsync(Filter);
    }

    public async Task<IActionResult> OnPostAsync(int expenseId, string action)
    {
        var request = new ApproveExpenseRequest
        {
            ExpenseId = expenseId,
            ReviewedBy = 2, // Bob Manager
            Approved = action == "approve"
        };

        await _expenseService.ApproveExpenseAsync(request);

        Message = action == "approve" ? "Expense approved successfully!" : "Expense rejected.";
        
        // Reload pending expenses
        PendingExpenses = await _expenseService.GetPendingExpensesAsync(Filter);
        
        return Page();
    }
}
