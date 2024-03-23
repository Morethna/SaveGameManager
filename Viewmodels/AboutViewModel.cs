using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Resources;
using SaveGameManager.Services;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace SaveGameManager.Viewmodels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel(IWindowService windowService)
    {
        OpenLinkCommand = new DelegateCommand(OpenUrl);
        WindowService = windowService;
    }
    public ICommand OpenLinkCommand { get; set; }

    public static string Version { get => $"Version: {Assembly.GetExecutingAssembly().GetName().Version}"; }
    private IWindowService WindowService { get; }

    private void OpenUrl(object obj)
    {
        string url = obj switch
        {
            "YouTube" => Hyperlinks.Youtube,
            "Discord" => Hyperlinks.Discord,
            _ => Hyperlinks.Twitch
        };
        try
        {
            Process.Start(new ProcessStartInfo { FileName = @$"{url}", UseShellExecute = true });
        }
        catch
        {

            WindowService.NotifierError($"Error while opening the {url}");
        }
    }
}
