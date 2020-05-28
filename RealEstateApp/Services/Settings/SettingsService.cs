using System.Linq;
using RealEstateApp.Models;
using RealEstateApp.Services.Repository;

namespace RealEstateApp.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        public SettingsService(IRepository repository)
        {
            var agent = repository.GetAgents().First();

            LoggedInUser = new User {Name = agent.Name, ImageUrl = agent.ImageUrl};
        }

        public User LoggedInUser { get; set; }
    }
}