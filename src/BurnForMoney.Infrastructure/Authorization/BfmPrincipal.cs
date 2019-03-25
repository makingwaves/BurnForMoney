using System;
using System.Collections.Generic;

namespace BurnForMoney.Infrastructure.Authorization
{
    public class BfmPrincipal
    {
        
        public Guid AadId { get; private set; }
        public string FirstName{ get; private set; }
        public string LastName { get; private set; }
        public List<KeyValuePair<string, string>> Claims { get; private set; }

        public bool IsAuthenticated { get; private set; }

        
        public static BfmPrincipal CreateAuthenticated(Guid id, string firstName, string lastName, List<KeyValuePair<string, string>> claims)
        {
            return new BfmPrincipal
            {
                IsAuthenticated = true,
                AadId = id,
                FirstName = firstName,
                LastName = lastName,
                Claims = claims
            };
        }

        public static BfmPrincipal CreateNotAuthenticated()
        {
            return new BfmPrincipal
            {
                IsAuthenticated = false,
                AadId = Guid.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Claims = new List<KeyValuePair<string, string>>()
            };
        }
    }    
}
