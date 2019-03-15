using System;
using Microsoft.Azure.WebJobs.Description;

namespace BurnForMoney.Infrastructure.Authorization.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class BfmAuthorizeAttribute : Attribute
    {}
}