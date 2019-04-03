using System.Linq;
using Microsoft.Extensions.Options;

namespace BurnForMoney.ApiGateway.Utils
{
    public interface IRedirectUriValidator
    {
        bool IsValid(string url);
        string GetDefaultIfNotValid(string url);
    }

    public class RedirectUriValidator : IRedirectUriValidator
    {
        private readonly AppConfiguration _configuration;

        
        public RedirectUriValidator(IOptions<AppConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public bool IsValid(string url)
        {
            return !string.IsNullOrEmpty(url) && (_configuration.ValidRedirectUris.Any(url.StartsWith) || url.StartsWith("/"));
        }

        public string GetDefaultIfNotValid(string url)
        {
            if (!IsValid(url))
                return _configuration.DefaultRedirectUri;

            return url;
        }
    }
}