using CatDex.Services.Interfaces;

namespace CatDex.Services;

public class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel)
    {
        await Shell.Current.DisplayAlertAsync(title, message, cancel);
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        return await Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
    }
}
