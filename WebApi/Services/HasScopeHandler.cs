using Microsoft.AspNetCore.Authorization;

namespace WebApi.Services
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        private readonly ILogger<HasScopeHandler> _logger;
        public HasScopeHandler(ILogger<HasScopeHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            _logger.LogInformation("Checking if 'permissions' claim exist");
            if (!context.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer))
            {
                _logger.LogInformation("'permissions' claim doesn't exist");
                return Task.CompletedTask;
            }

            //var scopes = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Value.Split(' ');

            //if (scopes.Any(s => s == requirement.Scope))
            //    context.Succeed(requirement);

            _logger.LogInformation("'permissions' exists. Checking if 'permissions' has correct value");
            var scopes = context.User.FindFirst(c => c.Type == "permissions" && c.Issuer == requirement.Issuer).Value;
            var permissions = scopes.Split(' ');
            if(permissions.Contains(requirement.Scope))
            {
                _logger.LogInformation("'permissions' has passed");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("'permissions' has wrong value");
            }

            return Task.CompletedTask;
        }
    }
}
