using SaveGameManagerMVVM.Interfaces;
using System.Windows.Forms;
using System.Windows.Input;
using SaveGameManagerMVVM.Core;
using System.Configuration;

namespace SaveGameManagerMVVM.Viewmodels;

public class TextDialogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private string _name = string.Empty;
    public TextDialogViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Name = _dataService.SelectedSaveGame?.Name ?? "";

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
        _dataService.SelectedSaveGame.Name = Name;
    }
}
