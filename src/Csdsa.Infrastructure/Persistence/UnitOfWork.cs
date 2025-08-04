using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Persistence.Repositories;
using Csdsa.Infrastructure.Repositories;
using Csdsa.Infrastructure.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Csdsa.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Users = new UserRepository(_context);
        // Roles = new RoleRepository(_context);
    }

    public IUserRepository Users { get; }

    // public IRoleRepository Roles { get; }

    public IGenericRepository<T> Repository<T>()
        where T : BaseEntity
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repo))
            return (IGenericRepository<T>)repo;

        var repository = new GenericRepository<T>(_context);
        _repositories[type] = repository;
        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started.");

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to roll back.");

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await _context.DisposeAsync();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
