using Application.Features.FavoriteRecipes.Command;
using Application.Features.FavoriteRecipes.Queries;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [Authorize(Policy = "active")]
    [ApiController]
    public class FavoritesRecipesController : BaseApiController
    {
        [HttpPost("{recipeId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Added", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> AddFavorite(string recipeId, CancellationToken cancellationToken)
        {
            var request = new CreateFavoriteRecipeCommand
            {
                RecipeId = Guid.Parse(recipeId)
            };

            var result = await Mediator.Send(request, cancellationToken);
            return result.ToOk();
        }
        [HttpDelete("{recipeId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe removed", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> RemoveFavorite(string recipeId, CancellationToken cancellationToken)
        {
            var request = new RemoveFavoriteByIdCommand()
            {
                RecipeId = Guid.Parse(recipeId)
            };

            var result = await Mediator.Send(request, cancellationToken);
            return result.ToOk();
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Added", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetFavorites(CancellationToken cancellationToken)
        {
            var query = new GetAllFavoritesRecipesQuery();
        
            var result = await Mediator.Send(query, cancellationToken);
            return result.ToOk();
        }
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Added", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetFavoritesId(CancellationToken cancellationToken)
        {
            var query = new GetAllFavoritesIdQuery();

            var result = await Mediator.Send(query, cancellationToken);
            return result.ToOk();
        }
    }
}
