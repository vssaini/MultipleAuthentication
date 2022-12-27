namespace MultipleAuthentication.Models
{
    public class AuthorizationConfig
    {
        public AzureAd AzureAd { get; set; }
        public Aws Aws { get; set; }
    }
}
