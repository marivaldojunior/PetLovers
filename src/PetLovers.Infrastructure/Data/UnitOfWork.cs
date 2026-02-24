using Microsoft.EntityFrameworkCore.Storage;
using PetLovers.Domain.Interfaces;
using PetLovers.Infrastructure.Data.Repositories;

namespace PetLovers.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly PetLoversDbContext _context;
    private IDbContextTransaction? _transaction;
    private IPetRepository? _petRepository;
    private IUserRepository? _userRepository;
    private bool _disposed;

    public UnitOfWork(PetLoversDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IPetRepository Pets => _petRepository ??= new PetRepository(_context);
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
