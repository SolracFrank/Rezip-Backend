using Application.Features.Recipes.Commands;
using Application.Features.Recipes.Validators;
using Domain.Dto;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class DeleteRecipeCommandHandler: IRequestHandler<DeleteRecipeCommand, Result<string>>
    {
        public DeleteRecipeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRecipeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRecipeCommandHandler> _logger;
        public async Task<Result<string>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating delete recipe request");

            var validator = new DeleteRecipeCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }
            _logger.LogInformation("Validating passed");

            _logger.LogInformation("Getting recipe");

            var recipe = await _unitOfWork.RecipeRepository.FindAsync(cancellationToken, request.Id);

            _logger.LogInformation("Getting recipe logo");

            var recipeLogo = await _unitOfWork.RecipeLogoRepository.FindAsync(cancellationToken, request.Id);

            _logger.LogInformation("Deleting recipe ");


            _unitOfWork.RecipeLogoRepository.Remove(recipeLogo!);
             _unitOfWork.RecipeRepository.Remove(recipe!);

             _logger.LogInformation("Deleting recipe");

            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

             if (!saved)
             {
                 _logger.LogInformation("Can not save changes");

                 return new Result<string>(new InfraestructureException("No fue posible borrar la receta."));
            }

             _logger.LogInformation("Recipe deleted");

            return new Result<string>("Eliminado satisfactoriamente");

        }
    }
}
