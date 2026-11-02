namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWalletRepository Wallets { get; }
    ICategoryRepository Categories { get; }
    ITransactionRepository Transactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

