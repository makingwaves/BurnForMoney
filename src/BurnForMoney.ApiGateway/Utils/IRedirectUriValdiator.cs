using System.Linq;
using Microsoft.Extensions.Options;

namespace BurnForMoney.ApiGateway.Utils
{
    public interface IRedirectUriValdiator
    {
        bool IsValid(string url);
        string GetDefaultIfNotValid(string url);
    }

    public class RedirectUriValdiator : IRedirectUriValdiator
    {
        private readonly AppConfiguration _configuration;

        public RedirectUriValdiator(IOptions<AppConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public bool IsValid(string url)
        {
            return !string.IsNullOrEmpty(url) && _configuration.ValidRedirectUris.Any(url.StartsWith);
        }

        public string GetDefaultIfNotValid(string url)
        {
            if (!IsValid(url))
                return _configuration.DefaultRedirectUri;

            return url;
        }
    }
}