using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    [Serializable]
    public class SqlConcurrencyException : DataException
    {
        public SqlConcurrencyException()
            : base ("Concurrency exception.")
        {
            
        }
    }
}