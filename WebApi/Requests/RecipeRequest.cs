namespace WebApi.Requests
{
    public class RecipeRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Procedures { get; set; } = null!;

        public IFormFile? Logo { get; set; }

    }
}
