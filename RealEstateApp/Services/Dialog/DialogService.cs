using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RealEstateApp.Services.Dialog
{
    public class DialogService : IDialogService
    {
        private static Page CurrentMainPage { get { return Application.Current.MainPage; } }

        public async Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            try
            {
                var displayButtons = buttons ?? new string[] { };
                var action = await CurrentMainPage.DisplayActionSheet(title, cancel, destruction, displayButtons);
                return action;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing action sheet - {ex}");
                return cancel;
            }
        }

        public async Task ShowAlertAsync(string message, string title, string buttonLabel = "OK")
        {
            try
            {
                await CurrentMainPage.DisplayAlert(title, message, buttonLabel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing alert - {ex}");
            }
        }
    }
}
