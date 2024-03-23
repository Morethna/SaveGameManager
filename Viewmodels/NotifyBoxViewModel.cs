namespace SaveGameManager.Viewmodels;

public class NotifyBoxViewModel() : ViewModelBase
{
    private string _message = string.Empty;
    private string _title = string.Empty;

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
}
