using System.Net.Mime;
using Application.Features.Recipes.Commands;
using Application.Features.Recipes.Queries;
using Domain.Dto;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Requests;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [Authorize(Policy = "active")]
    [ApiController]
    public class RecipesController : BaseApiController
    {
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Created", typeof(Recipe))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> CreateRecipe([FromForm] RecipeRequest request, CancellationToken cancellationToken)
        {
            var file = request.Logo;

            var command = new CreateRecipeCommand
            {
                Name = request.Name,
                Description = request.Description,
                Procedures = request.Procedures,
            };
            var contains = file != null;

            if (contains)
            {
                using var stream = new MemoryStream();

                await file!.CopyToAsync(stream, cancellationToken);

                var position = request.Logo!.FileName.LastIndexOf(".", StringComparison.Ordinal);
                var fileFormat = request.Logo.FileName[position..];

                var mimeType = file.ContentType;

                command.MimeType = mimeType;
                command.FileFormat = fileFormat;
                command.Logo = stream.ToArray();
            }


            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe updated", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> UpdaterRecipe([FromRoute]string id, [FromForm] RecipeRequest request, CancellationToken cancellationToken)
        {
            var file = request.Logo;

            var command = new UpdateRecipeCommand()
            {
                RecipeId = Guid.Parse(id),
                Name = request.Name,
                Description = request.Description,
                Procedures = request.Procedures,
            };
            var contains = file != null;

            if (contains)
            {
                using var stream = new MemoryStream();

                await file!.CopyToAsync(stream, cancellationToken);

                var position = request.Logo!.FileName.LastIndexOf(".", StringComparison.Ordinal);
                var fileFormat = request.Logo.FileName[position..];

                var mimeType = file.ContentType;

                command.MimeType = mimeType;
                command.FileFormat = fileFormat;
                command.Logo = stream.ToArray();
            }


            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipes Obtained", typeof(IEnumerable<RecipeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetAllUserRecipes(CancellationToken cancellationToken)
        {
            var query = new GetAllUserRecipesQuery();

            var result = await Mediator.Send(query, cancellationToken);

            return result.ToOk();
        }
        [HttpGet("general")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipes Obtained", typeof(IEnumerable<RecipeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetAllRecipes(CancellationToken cancellationToken)
        {
            var query = new GetAllRecipesQuery();

            var result = await Mediator.Send(query, cancellationToken);

            return result.ToOk();
        }
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Obtained", typeof(RecipeDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetRecipeById([FromRoute] string id, CancellationToken cancellationToken)
        {
            var query = new GetRecipeByIdQuery();
            query.id = Guid.Parse(id);

            var result = await Mediator.Send(query, cancellationToken);

            return result.ToOk();
        }

        [HttpGet("logo/{id}")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe logo Obtained", typeof(ContentDisposition))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> GetRecipeLogo([FromRoute] string id,CancellationToken cancellationToken)
        {
            var query = new GetRecipeLogoQuery();
            query.LogoId = Guid.Parse(id);
            var result = await Mediator.Send(query, cancellationToken);
            if (result.IsFaulted) return result.ToOk();

            var recipeLogo = result.Match(recipeLogo => recipeLogo, _ => null!);
            var fileName = $"{recipeLogo.FileName}{recipeLogo.FileFormat}";

            var contentDisposition = new ContentDisposition
            {
                FileName = fileName,
                Inline = true
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(recipeLogo.Logo!, recipeLogo.MimeType!);

        }
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recipe Deleted", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(BadRequestException))]
        public async Task<IActionResult> DeleteRecipe([FromRoute] string id, CancellationToken cancellationToken)
        {
            var command = new DeleteRecipeCommand();
            command.Id = Guid.Parse(id);
            var result = await Mediator.Send(command, cancellationToken);

            return result.ToOk();

        }
    }
}
