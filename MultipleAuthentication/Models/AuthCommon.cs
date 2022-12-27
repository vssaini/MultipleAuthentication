namespace MultipleAuthentication.Models
{
    public abstract class AuthCommon
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
