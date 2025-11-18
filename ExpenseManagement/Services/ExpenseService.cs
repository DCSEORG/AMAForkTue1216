using ExpenseManagement.Models;

namespace ExpenseManagement.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetExpensesAsync(string? filter = null);
    Task<List<Expense>> GetPendingExpensesAsync(string? filter = null);
    Task<Expense?> GetExpenseByIdAsync(int expenseId);
    Task<Expense> CreateExpenseAsync(CreateExpenseRequest request);
    Task<Expense> SubmitExpenseAsync(int expenseId);
    Task<Expense> ApproveExpenseAsync(ApproveExpenseRequest request);
    Task<List<ExpenseCategory>> GetCategoriesAsync();
    Task<List<User>> GetUsersAsync();
}

public class DummyExpenseService : IExpenseService
{
    private static List<Expense> _expenses = new()
    {
        new Expense
        {
            ExpenseId = 1,
            UserId = 1,
            UserName = "Alice Example",
            CategoryId = 1,
            CategoryName = "Travel",
            StatusId = 2,
            StatusName = "Submitted",
            Amount = 120.00m,
            Currency = "GBP",
            ExpenseDate = new DateTime(2024, 1, 15),
            Description = "Taxi from airport to client site",
            ReceiptFile = "/receipts/alice/taxi_jan15.jpg",
            SubmittedAt = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        },
        new Expense
        {
            ExpenseId = 2,
            UserId = 1,
            UserName = "Alice Example",
            CategoryId = 2,
            CategoryName = "Food",
            StatusId = 2,
            StatusName = "Submitted",
            Amount = 69.00m,
            Currency = "GBP",
            ExpenseDate = new DateTime(2023, 1, 10),
            Description = "Client lunch meeting",
            ReceiptFile = "/receipts/alice/lunch_jan10.jpg",
            SubmittedAt = DateTime.UtcNow.AddDays(-10),
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        },
        new Expense
        {
            ExpenseId = 3,
            UserId = 1,
            UserName = "Alice Example",
            CategoryId = 3,
            CategoryName = "Office Supplies",
            StatusId = 3,
            StatusName = "Approved",
            Amount = 99.50m,
            Currency = "GBP",
            ExpenseDate = new DateTime(2023, 12, 4),
            Description = "Office stationery and supplies",
            ReceiptFile = "/receipts/alice/supplies_dec04.jpg",
            SubmittedAt = DateTime.UtcNow.AddDays(-30),
            ReviewedBy = 2,
            ReviewedAt = DateTime.UtcNow.AddDays(-28),
            CreatedAt = DateTime.UtcNow.AddDays(-31)
        },
        new Expense
        {
            ExpenseId = 4,
            UserId = 1,
            UserName = "Alice Example",
            CategoryId = 4,
            CategoryName = "Transport",
            StatusId = 3,
            StatusName = "Approved",
            Amount = 19.20m,
            Currency = "GBP",
            ExpenseDate = new DateTime(2023, 21, 18),
            Description = "Train tickets for client visit",
            ReceiptFile = "/receipts/alice/transport_sep18.jpg",
            SubmittedAt = DateTime.UtcNow.AddDays(-60),
            ReviewedBy = 2,
            ReviewedAt = DateTime.UtcNow.AddDays(-58),
            CreatedAt = DateTime.UtcNow.AddDays(-61)
        }
    };

    private static List<ExpenseCategory> _categories = new()
    {
        new ExpenseCategory { CategoryId = 1, CategoryName = "Travel", IsActive = true },
        new ExpenseCategory { CategoryId = 2, CategoryName = "Meals", IsActive = true },
        new ExpenseCategory { CategoryId = 3, CategoryName = "Supplies", IsActive = true },
        new ExpenseCategory { CategoryId = 4, CategoryName = "Accommodation", IsActive = true },
        new ExpenseCategory { CategoryId = 5, CategoryName = "Other", IsActive = true }
    };

    private static List<User> _users = new()
    {
        new User
        {
            UserId = 1,
            UserName = "Alice Example",
            Email = "alice@example.co.uk",
            RoleId = 1,
            RoleName = "Employee",
            ManagerId = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        },
        new User
        {
            UserId = 2,
            UserName = "Bob Manager",
            Email = "bob.manager@example.co.uk",
            RoleId = 2,
            RoleName = "Manager",
            ManagerId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-12)
        }
    };

    public Task<List<Expense>> GetExpensesAsync(string? filter = null)
    {
        var expenses = _expenses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            expenses = expenses.Where(e =>
                e.CategoryName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                e.Description?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true ||
                e.StatusName.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(expenses.OrderByDescending(e => e.ExpenseDate).ToList());
    }

    public Task<List<Expense>> GetPendingExpensesAsync(string? filter = null)
    {
        var expenses = _expenses.Where(e => e.StatusName == "Submitted");

        if (!string.IsNullOrWhiteSpace(filter))
        {
            expenses = expenses.Where(e =>
                e.CategoryName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                e.Description?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true);
        }

        return Task.FromResult(expenses.OrderBy(e => e.SubmittedAt).ToList());
    }

    public Task<Expense?> GetExpenseByIdAsync(int expenseId)
    {
        var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
        return Task.FromResult(expense);
    }

    public Task<Expense> CreateExpenseAsync(CreateExpenseRequest request)
    {
        var user = _users.FirstOrDefault(u => u.UserId == request.UserId);
        var category = _categories.FirstOrDefault(c => c.CategoryId == request.CategoryId);

        var expense = new Expense
        {
            ExpenseId = _expenses.Max(e => e.ExpenseId) + 1,
            UserId = request.UserId,
            UserName = user?.UserName ?? "Unknown",
            CategoryId = request.CategoryId,
            CategoryName = category?.CategoryName ?? "Unknown",
            StatusId = 1,
            StatusName = "Draft",
            Amount = request.Amount,
            Currency = "GBP",
            ExpenseDate = request.ExpenseDate,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _expenses.Add(expense);
        return Task.FromResult(expense);
    }

    public Task<Expense> SubmitExpenseAsync(int expenseId)
    {
        var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
        if (expense == null)
            throw new Exception("Expense not found");

        expense.StatusId = 2;
        expense.StatusName = "Submitted";
        expense.SubmittedAt = DateTime.UtcNow;

        return Task.FromResult(expense);
    }

    public Task<Expense> ApproveExpenseAsync(ApproveExpenseRequest request)
    {
        var expense = _expenses.FirstOrDefault(e => e.ExpenseId == request.ExpenseId);
        if (expense == null)
            throw new Exception("Expense not found");

        expense.StatusId = request.Approved ? 3 : 4;
        expense.StatusName = request.Approved ? "Approved" : "Rejected";
        expense.ReviewedBy = request.ReviewedBy;
        expense.ReviewedAt = DateTime.UtcNow;

        return Task.FromResult(expense);
    }

    public Task<List<ExpenseCategory>> GetCategoriesAsync()
    {
        return Task.FromResult(_categories.Where(c => c.IsActive).ToList());
    }

    public Task<List<User>> GetUsersAsync()
    {
        return Task.FromResult(_users.Where(u => u.IsActive).ToList());
    }
}
