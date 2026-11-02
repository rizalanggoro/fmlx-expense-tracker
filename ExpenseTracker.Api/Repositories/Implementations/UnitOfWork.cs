using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Repositories.Interfaces;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IWalletRepository? _wallets;
    private ICategoryRepository? _categories;
    private ITransactionRepository? _transactions;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ITransactionRepository Transactions => _transactions ??= new TransactionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
