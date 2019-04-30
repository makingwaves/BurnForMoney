using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class FailedToCreateAccountException : DataException
    {
        public FailedToCreateAccountException(string message) : base(message)
        {
        }

        public FailedToCreateAccountException(string athleteId, string activeDirectoryId)
            : base($"Failed to create account for athlete: [{athleteId}] with active directory id: [{activeDirectoryId}]")
        {
        }

        protected FailedToCreateAccountException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
