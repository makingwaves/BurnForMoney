using System;
using Microsoft.Azure.WebJobs.Description;

namespace BurnForMoney.Functions.Shared.Functions.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class ConfigurationAttribute : Attribute
    {

    }
}