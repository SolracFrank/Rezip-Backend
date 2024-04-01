namespace Infrastructure.Repositories;

using Data;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

public class UnitOfWork : IUnitOfWork
{
    private readonly RecipesDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;


    public UnitOfWork(RecipesDbContext context, ILogger<UnitOfWork> logger,
     IRepository<User> userRepository,
     IRepository<Recipe> recipeRepository)
    {
        _context = context;
        _logger = logger;
        UserRepository = userRepository;
        RecipeRepository = recipeRepository;
    }



    public IRepository<User> UserRepository { get; }
    public IRepository<Recipe> RecipeRepository { get; }


    public bool SaveChanges()
    {
        try
        {
            _logger.LogInformation("Commiting changes.");

            var saved = _context.SaveChanges();

            _logger.LogInformation("Saved changes: {saved}", saved);

            return saved > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception has ocurred {Message}.", ex.Message);

            throw new InfraestructureException(ex.Message);
        }
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Commiting changes.");

            var saved = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Saved changes: {saved}", saved);

            return saved > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception has ocurred {Message}.", ex.Message);

            throw new InfraestructureException(ex.Message);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

