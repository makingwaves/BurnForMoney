using System;

namespace BurnForMoney.Functions.Exceptions
{
    public class SystemNotSupportedException : NotSupportedException
    {
        public SystemNotSupportedException(string systemName)
            : base($"System: [{systemName}] is currently not supported.")
        {
        }
    }
}