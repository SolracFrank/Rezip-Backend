using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace Infrastructure;
using Data;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjections
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ProblemDetailsExtensions.AddProblemDetails(services);

        services.AddDbContext<RecipesDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DbConn"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"),
                b => b.EnableRetryOnFailure(4, TimeSpan.FromSeconds(5), null))
            );

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<IUnitOfWork,UnitOfWork>();
        services.AddTransient<IUserService, UserService>();


    }
}
