namespace Domain.Custom
{
    public class Auth0Config
    {
        public string ClientSecrets { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string Domain { get; set; } = null!;
        public string GrantType { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Authority { get; set; } = null!;
        public string ApiIdentifier { get; set; } = null!;
    }
}
