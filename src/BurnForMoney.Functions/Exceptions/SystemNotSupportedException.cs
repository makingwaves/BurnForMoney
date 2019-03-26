using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class SystemNotSupportedException : NotSupportedException
    {
        public SystemNotSupportedException(string systemName)
            : base($"System: [{systemName}] is currently not supported.")
        {
        }

        protected SystemNotSupportedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}