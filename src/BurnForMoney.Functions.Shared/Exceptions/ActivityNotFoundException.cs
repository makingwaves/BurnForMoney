using System;

namespace BurnForMoney.Functions.Shared.Exceptions
{
    public class ActivityNotFoundException : Exception 
    {
        public ActivityNotFoundException(string id)
            : base($"Activity with id: {id} does not exists.")
        {

        }
    }
}