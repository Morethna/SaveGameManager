using SaveGameManagerMVVM.Interfaces;
using System.Windows.Forms;
using System.Windows.Input;
using SaveGameManagerMVVM.Core;

namespace SaveGameManagerMVVM.Viewmodels;

public class TextDialogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IWindowService _windowService;

    public TextDialogViewModel(IDataService dataService, IWindowService windowService)
    {
        _dataService = dataService;
        _windowService = windowService;
        SetNameCommand = new DelegateCommand(SetName);
    }
    public ICommand SetNameCommand { get; set; }
    public string Name 
    { 
        get => _dataService.SelectedSaveGame?.Name ?? "";
        set 
        {
            if (_dataService.SelectedSaveGame?.Name == value)
                return;

            _dataService.SelectedSaveGame.Name = value;
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
        _dataService.SelectedSaveGame.Name = Name;
        _windowService.CloseWindow(IWindowService.Windows.Textdialog);
    }
}
