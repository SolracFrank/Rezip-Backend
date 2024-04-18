namespace Infrastructure.Services;

using Data;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Threading;

public class Repository<T> : IRepository<T> where T : class
{
    private const string DefaultDatabaseErrorMessage = "Communication error with database has ocurred.";
    protected readonly RecipesDbContext Context;
    protected readonly DbSet<T> Entities;
    protected readonly ILogger<Repository<T>> Logger;

    public Repository(RecipesDbContext context, ILogger<Repository<T>> logger)
    {
        Context = context;
        Logger = logger;
        Entities = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await Entities.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        await Entities.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            return await Entities.AllAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            return await Entities.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken, bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.FirstOrDefaultAsync(predicate, cancellationToken)
                : await Entities.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.ToListAsync(cancellationToken)
                : await Entities.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }

    public void Remove(T entity)
    {
        Entities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        Entities.RemoveRange(entities);
    }

    public void Update(T entity)
    {
        Entities.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        Entities.UpdateRange(entities);
    }

    public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken, bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.Where(predicate).ToListAsync(cancellationToken)
                : await Entities.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<T?> FindAsync(CancellationToken cancellationToken, params object[] id)
    {
        return await Entities.FindAsync(id, cancellationToken);
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return  Entities.Where(predicate);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred {Message}.", ex.Message);

            throw new InfraestructureException(DefaultDatabaseErrorMessage);
        }
    }
}
