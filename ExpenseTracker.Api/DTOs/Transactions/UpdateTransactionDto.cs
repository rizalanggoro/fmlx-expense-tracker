namespace ExpenseTracker.Api.DTOs.Transactions;

public record UpdateTransactionDto(
    decimal Amount,
    DateTime TransactionDate,
    string? Description
);