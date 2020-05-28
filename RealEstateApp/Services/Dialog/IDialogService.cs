
using System.Threading.Tasks;

namespace RealEstateApp.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel = "OK");
        Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
    }
}
