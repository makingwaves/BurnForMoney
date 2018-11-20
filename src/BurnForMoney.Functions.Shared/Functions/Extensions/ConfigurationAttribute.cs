using System;
using Microsoft.Azure.WebJobs.Description;

namespace BurnForMoney.Functions.Shared.Functions.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class ConfigurationAttribute : Attribute
    {

    }
}