namespace Domain.Interfaces;
using Entities;



public interface IUnitOfWork : IDisposable
{
    IRepository<Recipe> RecipeRepository { get; }

    IRepository<User> UserRepository { get; }
    IRepository<RecipeLogo> RecipeLogoRepository { get; }

    IRepository<UserFavoritesRecipe> UserFavoriteRepository { get; }
    IRepository<Comment> CommentRepository { get; }


    bool SaveChanges();

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}