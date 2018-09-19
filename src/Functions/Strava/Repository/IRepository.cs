using System.Threading.Tasks;

namespace BurnForMoney.Functions.Strava.Repository
{
    public interface IRepository
    {
        Task BootstrapAsync();
    }
}