using ExpenseManagement.Models;
using ExpenseManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseManagement.Pages;

public class AddExpenseModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public AddExpenseModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public List<ExpenseCategory> Categories { get; set; } = new();
    
    [BindProperty]
    public decimal Amount { get; set; }
    
    [BindProperty]
    public DateTime ExpenseDate { get; set; }
    
    [BindProperty]
    public int CategoryId { get; set; }
    
    [BindProperty]
    public string? Description { get; set; }

    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _expenseService.GetCategoriesAsync();
        ExpenseDate = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Categories = await _expenseService.GetCategoriesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var request = new CreateExpenseRequest
        {
            UserId = 1, // Alice Example
            CategoryId = CategoryId,
            Amount = Amount,
            ExpenseDate = ExpenseDate,
            Description = Description
        };

        var expense = await _expenseService.CreateExpenseAsync(request);
        
        // Auto-submit the expense
        await _expenseService.SubmitExpenseAsync(expense.ExpenseId);

        Message = "Expense submitted successfully!";
        
        // Redirect to Index page
        return RedirectToPage("/Index");
    }
}
