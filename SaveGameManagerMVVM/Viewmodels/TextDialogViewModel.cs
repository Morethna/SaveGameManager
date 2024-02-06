using SaveGameManagerMVVM.Interfaces;
using System.Windows.Forms;
using System.Windows.Input;
using SaveGameManagerMVVM.Core;

namespace SaveGameManagerMVVM.Viewmodels;

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
            MessageBox.Show("Name can't be empty!");
            return;
        }
        _windowService.CloseWindow(IWindowService.Windows.Textdialog);
    }
}
