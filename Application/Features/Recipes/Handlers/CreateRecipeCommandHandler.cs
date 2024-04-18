using Application.Features.Recipes.Commands;
using Application.Features.Recipes.Validators;
using Domain.Custom;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result<RecipeDto>>
    {
        public CreateRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRecipeCommandHandler> logger, IUserService userService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
            _logosTypes = configuration.GetSection("MymeType").Get<List<RecipeLogosTypes>>()!;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRecipeCommandHandler> _logger;
        private readonly IUserService _userService;
        private readonly List<RecipeLogosTypes> _logosTypes;
        public async Task<Result<RecipeDto>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<RecipeDto>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            var user = userRequest.Match(user => user, _ => null!);
            request.CreatedBy = user.UserId;

            var validator = new CreateRecipeCommandValidator(_unitOfWork, _logosTypes);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<RecipeDto>(validationException);
            }
            var id = Guid.NewGuid();
            var recipe = new Recipe()
            {
                RecipeId = id,
                Description = request.Description,
                Procedures = request.Procedures,
                Name = request.Name,
                CreatedBy = request.CreatedBy,
                RecipeLogo = new RecipeLogo
                {
                    RecipeId = id,
                    FileFormat = request.FileFormat,
                    MimeType = request.MimeType,
                    Logo = request.Logo
                }
            };

            await _unitOfWork.RecipeRepository.AddAsync(recipe, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<RecipeDto>(new InfraestructureException("No fue posible crear la receta."));
            }
            _logger.LogInformation("Receta created.");

            return new Result<RecipeDto>(recipe.ToRecipeDto());
        }
    }
}
