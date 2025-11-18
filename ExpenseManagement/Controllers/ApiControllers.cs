using ExpenseManagement.Models;
using ExpenseManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Get all expenses with optional filter
    /// </summary>
    /// <param name="filter">Optional filter text for category, description, or status</param>
    /// <returns>List of expenses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<Expense>), 200)]
    public async Task<ActionResult<List<Expense>>> GetExpenses([FromQuery] string? filter = null)
    {
        var expenses = await _expenseService.GetExpensesAsync(filter);
        return Ok(expenses);
    }

    /// <summary>
    /// Get pending expenses (Submitted status) with optional filter
    /// </summary>
    /// <param name="filter">Optional filter text</param>
    /// <returns>List of pending expenses</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<Expense>), 200)]
    public async Task<ActionResult<List<Expense>>> GetPendingExpenses([FromQuery] string? filter = null)
    {
        var expenses = await _expenseService.GetPendingExpensesAsync(filter);
        return Ok(expenses);
    }

    /// <summary>
    /// Get a specific expense by ID
    /// </summary>
    /// <param name="id">Expense ID</param>
    /// <returns>Expense details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Expense), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
            return NotFound();

        return Ok(expense);
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    /// <param name="request">Expense details</param>
    /// <returns>Created expense</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Expense), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than zero");

        var expense = await _expenseService.CreateExpenseAsync(request);
        return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, expense);
    }

    /// <summary>
    /// Submit an expense for approval
    /// </summary>
    /// <param name="id">Expense ID</param>
    /// <returns>Updated expense</returns>
    [HttpPost("{id}/submit")]
    [ProducesResponseType(typeof(Expense), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Expense>> SubmitExpense(int id)
    {
        try
        {
            var expense = await _expenseService.SubmitExpenseAsync(id);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Approve or reject an expense
    /// </summary>
    /// <param name="request">Approval details</param>
    /// <returns>Updated expense</returns>
    [HttpPost("approve")]
    [ProducesResponseType(typeof(Expense), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Expense>> ApproveExpense([FromBody] ApproveExpenseRequest request)
    {
        try
        {
            var expense = await _expenseService.ApproveExpenseAsync(request);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public CategoriesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Get all active expense categories
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseCategory>), 200)]
    public async Task<ActionResult<List<ExpenseCategory>>> GetCategories()
    {
        var categories = await _expenseService.GetCategoriesAsync();
        return Ok(categories);
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public UsersController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Get all active users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), 200)]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await _expenseService.GetUsersAsync();
        return Ok(users);
    }
}
