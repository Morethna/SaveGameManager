using SaveGameManager.Interfaces;
using System.Windows.Forms;
using System.Windows.Input;
using SaveGameManager.Core;

namespace SaveGameManager.Viewmodels;

public class TextDialogViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    private string _name = string.Empty;

    public TextDialogViewModel(IWindowService windowService)
    {
        _windowService = windowService;
        SetNameCommand = new DelegateCommand(SetName);
    }
    public ICommand SetNameCommand { get; set; }

    public bool Ok { get; private set; } = false;
    public string Name 
    { 
        get => _name;
        set 
        {
            if (_name == value)
                return;

            _name = value;
            OnPropertyChanged(nameof(Name));
        } 
    }

    private void SetName(object obj)
    {
        if (string.IsNullOrEmpty(Name))
        {
            _windowService.NotifierWarning("Name can't be empty!");
            return;
        }
        Ok = true;
        _windowService.CloseWindow(IWindowService.Windows.Textdialog);
    }
}
