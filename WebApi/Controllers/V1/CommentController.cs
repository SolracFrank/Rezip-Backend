using Application.Features.Comments.Commands;
using Application.Features.Comments.Queries;
using Domain.Dto;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "active")]
    public class CommentController: BaseApiController
    {
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment Created", typeof(Comment))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return result.ToOk();
        }
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment Got", typeof(IEnumerable<CommentDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetAllCommentsByRecipe([FromRoute] string id, CancellationToken cancellationToken)
        {
            var query = new GetCommentsByRecipesQuery
            {
                RecipeId = Guid.Parse(id)
            };
                
            var result = await Mediator.Send(query, cancellationToken);
            return result.ToOk();
        }
        [HttpDelete("{commentId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment deleted", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> DeleteCommentByUser([FromRoute] string commentId, CancellationToken cancellationToken)
        {
            var command = new DeleteCommentByUserCommand
            {
                CommentId = Guid.Parse(commentId)
            };

            var result = await Mediator.Send(command, cancellationToken);
            return result.ToOk();
        }

        [HttpDelete("recipe/{recipeId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comment deleted", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> DeleteCommentByRecipe([FromRoute] string recipeId,[FromBody] DeleteCommentByRecipeIdCommand command, CancellationToken cancellationToken)
        {
            command.RecipeId = Guid.Parse(recipeId);

             var result = await Mediator.Send(command, cancellationToken);
            return result.ToOk();
        }
    }
}
