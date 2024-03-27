using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using  Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class DependencyInjections
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ProblemDetailsExtensions.AddProblemDetails(services);
    }
}
