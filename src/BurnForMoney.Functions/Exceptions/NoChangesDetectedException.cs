using System;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Exceptions
{
    [Serializable]
    public class NoChangesDetectedException : InvalidOperationException
    {
        public NoChangesDetectedException()
            : base("Update operation must change at least one field. No changes detected.")
        {
        }

        protected NoChangesDetectedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}