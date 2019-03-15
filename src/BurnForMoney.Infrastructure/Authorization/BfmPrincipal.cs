namespace BurnForMoney.Infrastructure.Authorization
{
    public class BfmPrincipal
    {
        
        public string Id { get; private set; }
        public string FirstName{ get; private set; }
        public string LastName { get; private set; }

        public bool IsAuthenticated { get; private set; }



        public static BfmPrincipal CreateAuthenticated(string id)
        {
            return new BfmPrincipal
            {
                IsAuthenticated = true,
                Id = id
            };
        }

        public static BfmPrincipal CreateNotAuthenticated()
        {
            return new BfmPrincipal
            {
                IsAuthenticated = false
            };
        }
    }    
}
