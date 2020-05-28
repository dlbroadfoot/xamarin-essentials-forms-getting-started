using RealEstateApp.Models;

namespace RealEstateApp.Services.Settings
{
    public interface ISettingsService
    {
        User LoggedInUser { get; set; }
    }
}