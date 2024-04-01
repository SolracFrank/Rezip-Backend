namespace Domain.Interfaces;

using Entities;

public interface IUnitOfWork : IDisposable
{
    IRepository<Recipe> RecipeRepository { get; }

    IRepository<User> UserRepository { get; }

    bool SaveChanges();

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}