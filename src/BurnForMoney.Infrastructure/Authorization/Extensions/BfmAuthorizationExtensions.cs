using System;
using Microsoft.Azure.WebJobs;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    public static class BfmAuthorizationExtensions
    {
        public static IWebJobsBuilder AddBfmAuthorization(this IWebJobsBuilder builder, IBfmPrincipalFactory factory)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddExtension(new BfmAuthorizationExtensionProvider(factory));
            return builder;
        }
    }
}