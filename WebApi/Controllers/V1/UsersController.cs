using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(IEnumerable<UserDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var query = new GetUsersQuery();
            var result = await Mediator.Send(query, cancellationToken);

            return result.ToOk();
        }
        [HttpGet("{UserId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> GetUserById([FromRoute] GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(query, cancellationToken);

            return result.ToOk();
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }

        [HttpDelete("{UserId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> CreateUser([FromRoute] DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }
        [HttpPatch("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "User", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> UpdateUser([FromRoute]Guid id,[FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            command.UserId = id;

            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }
    }
}