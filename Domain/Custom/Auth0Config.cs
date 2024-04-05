namespace Domain.Custom
{
    public class Auth0Config
    {
        public string ClientSecrets { get; set; }
        public string ClientId { get; set; }
        public string Domain { get; set; }
        public string GrantType { get; set; }
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string ApiIdentifier { get; set; }
    }
}
