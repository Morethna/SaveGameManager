using SaveGameManager.Core;
using System.Diagnostics;
using System.Windows.Input;

namespace SaveGameManager.Viewmodels;

public class GitHubViewModel : ViewModelBase
{
    private string _newVersion = string.Empty;
    private string _currVersion = string.Empty;
    public GitHubViewModel() => OpenUrlCommand = new DelegateCommand(OpenUrl);
    public ICommand OpenUrlCommand { get; set; }

    public string? Url { get; set; }
    public string NewVersion { get => $"New Version:\n{_newVersion}"; set => _newVersion = value; }
    public string CurrentVersion { get => $"Current Version:\n{_currVersion}"; set => _currVersion = value; }

    private void OpenUrl(object obj)
    {
        if (Url == null)
            return;

        Process.Start(new ProcessStartInfo { FileName = @$"{Url}", UseShellExecute = true });
    }


}
