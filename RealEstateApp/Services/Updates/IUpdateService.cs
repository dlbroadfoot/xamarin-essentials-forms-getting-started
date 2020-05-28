
using System.Threading.Tasks;

namespace RealEstateApp.Services.Updates
{
    public interface IUpdateService
    {
        /// <summary>
        /// Checks the server to see if any updates to the app are available
        /// </summary>
        /// <returns>True, if updates are available</returns>
        Task<bool> CheckForAppUpdatesAsync();
    }
}
