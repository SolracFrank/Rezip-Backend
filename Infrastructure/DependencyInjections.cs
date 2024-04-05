namespace Infrastructure;
using Data;
using Domain.Custom;
using Domain.Interfaces;
using Hellang.Middleware.ProblemDetails;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjections
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ProblemDetailsExtensions.AddProblemDetails(services);
        services.Configure<Auth0Claims>(configuration.GetSection("Auth0:Claims"));
        services.Configure<Auth0Config>(configuration.GetSection("Auth0"));

        services.AddDbContext<RecipesDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DbConn"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"),
                b => b.EnableRetryOnFailure(4, TimeSpan.FromSeconds(5), null))
            );

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAuth0Service, Auth0Service>();

    }
}
