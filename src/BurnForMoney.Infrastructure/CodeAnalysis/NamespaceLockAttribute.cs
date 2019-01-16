using System;

namespace BurnForMoney.Infrastructure.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NamespaceLockAttribute : Attribute
    {
        public const string Public_Contract_Please_Do_Not_Change_Its_Namespace =
            "Public_Contract_Please_Do_Not_Change_Its_Namespace";

        public string Reason { get; set; }
    }
}