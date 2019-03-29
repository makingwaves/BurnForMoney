using System;

namespace BurnForMoney.ApiGateway.Utils
{
    public class OidcConfiguration
    {
        public string[] AcceptableClientsIds { get; set; }

        public string SigningCredentialsSymmetricKey { get; set; }
        public string AuthorizationEndpointPath { get; set; }
        public string UserinfoEndpointPath { get; set; }
    }

    public class DataProtectionConfiguration
    {
        public string KeyPersistanceBlobName { get; set; }
        public Uri KeyPersistanceBlobAddress { get; set; }
        public string KeysProtectionKeyVault { get; set; }
    }
    
    public class AppConfiguration
    {
        public Uri PublicApiUri { get; set; }
        public Uri InternalApiUri { get; set; }
        public Uri StravaApiUri { get; set; }

        public string PublicApiMasterKey { get; set; }
        public string InternalApiMasterKey { get; set; }
        public string StravaApiUriMasterKey { get; set; }

        public string StravaAuthorizationUrl { get; set; }
        public string StravaAuthorizationRedirectUrl { get; set; }

        public string[] ValidRedirectUris { get; set; }
        public string DefaultRedirectUri { get; set; }
    }
}