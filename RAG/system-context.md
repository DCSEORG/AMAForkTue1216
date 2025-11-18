# Expense Management System - Context Information

## System Overview
The Expense Management System is designed to help employees submit and track their business expenses. Managers can review and approve or reject submitted expenses.

## User Roles

### Employee
- Can create and submit expenses
- View their own expense history
- Track status of submitted expenses
- Categories include: Travel, Meals, Supplies, Accommodation, Other

### Manager
- Can review pending expenses
- Approve or reject employee expenses
- View all expenses in the system

## Expense Workflow

1. **Draft**: Employee creates an expense
2. **Submitted**: Employee submits for manager review
3. **Approved/Rejected**: Manager makes decision

## Business Rules

- All amounts are in GBP (British Pounds)
- Expenses require: amount, date, category, and optional description
- Receipts can be attached (file path reference)
- Managers typically review expenses within 2-3 business days

## Common Expense Categories

- **Travel**: Transportation costs, mileage, parking
- **Meals**: Client lunches, team dinners
- **Supplies**: Office materials, equipment
- **Accommodation**: Hotel stays during business trips
- **Other**: Miscellaneous business expenses

## API Endpoints

- GET /api/expenses - List all expenses
- GET /api/expenses/pending - List pending approvals
- POST /api/expenses - Create new expense
- POST /api/expenses/{id}/submit - Submit for approval
- POST /api/expenses/approve - Approve/reject expense
- GET /api/categories - List expense categories

## Tips for Using the Chat Assistant

- Ask natural language questions about expenses
- Request to create new expenses with specific amounts and categories
- Inquire about pending approvals
- Get summaries of expense data

Example queries:
- "Show me all travel expenses"
- "Create a new expense for £45.50 for office supplies"
- "What expenses are pending approval?"
- "List all approved expenses from this month"
