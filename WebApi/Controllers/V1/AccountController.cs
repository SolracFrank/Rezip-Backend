using Application.Features.Account.Commands;
using Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "authenticated")]
    public class AccountController : BaseApiController
    {

        [HttpPost("register")]
        [SwaggerResponse(StatusCodes.Status200OK, "Create", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RegisterUser(CancellationToken cancellationToken)
        {
            var command = new RegisterCommand();
            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }
    }
}
