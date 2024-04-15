using SaveGameManager.Core;
using System;

namespace SaveGameManager.Models;
public class Savegame : OberservableObject
{
    private string _name = string.Empty;
    private string _path = string.Empty;
    private DateTime _creationDate;
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
    public string Path
    {
        get => _path;
        set
        {
            if (_path == value)
                return;

            _path = value;
            OnPropertyChanged(nameof(Path));
        }
    }
    public DateTime CreationDate
    {
        get => _creationDate;
        set
        {
            if (_creationDate == value)
                return;

            _creationDate = value;
            OnPropertyChanged(nameof(CreationDate));
        }
    }
}
