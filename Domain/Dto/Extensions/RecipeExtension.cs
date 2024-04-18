using Domain.Entities;

namespace Domain.Dto.Extensions
{
    public static class RecipeExtension
    {
        public static RecipeDto ToRecipeDto(this Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe?.RecipeId,
                Name = recipe?.Name,
                Description = recipe?.Description,
                Procedures = recipe?.Procedures,
                CreatedBy = recipe?.CreatedByNavigation?.Username,
            };
        }
        public static IEnumerable<RecipeDto> ToRecipeDto(this IEnumerable<Recipe> files)
        {
            return files.Select(file => file.ToRecipeDto());
        }
    }
}
