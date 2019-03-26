using System;
using System.Data;
using System.Runtime.Serialization;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class SqlConcurrencyException : DataException
    {
        public SqlConcurrencyException()
            : base("Concurrency exception.")
        {
        }

        protected SqlConcurrencyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}