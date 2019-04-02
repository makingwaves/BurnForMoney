using System;
using System.Data;

namespace BurnForMoney.Functions.Presentation.Exceptions
{
    public class FailedToAssignActiveDirectoryIdException : DataException
    {
        public FailedToAssignActiveDirectoryIdException(Guid athleteId) : base(
            $"Failed to assign Active Directory id to athlete: [{athleteId}].")
        {
        }
    }
}