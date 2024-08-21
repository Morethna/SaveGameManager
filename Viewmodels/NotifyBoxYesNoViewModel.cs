using SaveGameManager.Interfaces;
using System.Windows.Forms;
using System.Windows.Input;
using SaveGameManager.Core;

namespace SaveGameManager.Viewmodels;

public class NotifyBoxYesNoViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    private string _message = string.Empty;
    private string _title = string.Empty;

    public NotifyBoxYesNoViewModel(IWindowService windowService)
    {
        _windowService = windowService;
        OkResultCommand = new DelegateCommand(OkResult);
        FocusCommand = new DelegateCommand(ResetResult);
    }
    public ICommand OkResultCommand { get; set; }
    public ICommand FocusCommand { get; set; }

    public bool Result { get; private set; } = false;
    public string Title
    {
        get => _title;
        set
        {
            if (_title == value)
                return;
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }
    public string Message
    { 
        get => _message;
        set 
        {
            if (_message == value)
                return;
            _message = value;
            OnPropertyChanged(nameof(Message));
        } 
    }
    private void ResetResult(object obj) => Result = false;

    private void OkResult(object obj)
    {
        Result = true;
        _windowService.CloseWindow(this);
    }
}
