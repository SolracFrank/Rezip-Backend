using Application.Features.Recipes.Commands;
using Application.Features.Recipes.Validators;
using Domain.Custom;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class UpdateRecipeCommandHandler :IRequestHandler<UpdateRecipeCommand, Result<string>>
    {
        public UpdateRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRecipeCommandHandler> logger, IUserService userService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
            _logosTypes = configuration.GetSection("MymeType").Get<List<RecipeLogosTypes>>()!;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRecipeCommandHandler> _logger;
        private readonly IUserService _userService;
        private readonly List<RecipeLogosTypes> _logosTypes;
        public async Task<Result<string>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
        {
            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<string>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            _logger.LogInformation("Validating request");

            var validator = new UpdateRecipeCommandValidator(_unitOfWork, _logosTypes);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }

            _logger.LogInformation("Getting recipe");

            var recipe = await _unitOfWork.RecipeRepository.FindAsync(cancellationToken, request.RecipeId);
            _logger.LogInformation("Getting logo");


            recipe!.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Procedures = request.Procedures;

            _logger.LogInformation("Checking if is logo in the request");

            var logoExists = request.Logo != null;

            if (logoExists)
            {
                var recipeLogo = await _unitOfWork.RecipeLogoRepository.FindAsync(cancellationToken, request.RecipeId);

                _logger.LogInformation("Logo exists, updating logo");

                recipeLogo!.Logo = request.Logo;
                recipeLogo.FileFormat = request.FileFormat!;
                recipeLogo.MimeType = request.MimeType!;
                _unitOfWork.RecipeLogoRepository.Update(recipeLogo);
            }
            _logger.LogInformation("Logo doesn't exist, updating recipe only");

            _unitOfWork.RecipeRepository.Update(recipe);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                _logger.LogInformation("Update successful");

                return new Result<string>("Cambios guardados satisfactoriamente");
            }
            _logger.LogInformation("Update failed");

            return new Result<string>(new InfraestructureException("Error al actualizar recipe"));

        }
    }
}
