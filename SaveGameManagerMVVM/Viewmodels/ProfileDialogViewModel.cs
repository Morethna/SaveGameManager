using SaveGameManagerMVVM.Interfaces;
using SaveGameManagerMVVM.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveGameManagerMVVM.Viewmodels;
public class ProfileDialogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly ISettingsService _settingsService;
    private readonly IDirectoryService _directoryService;
    private readonly IWindowService _windowService;

    public ProfileDialogViewModel(IDataService dataService,
        ISettingsService settingsService,
        IDirectoryService directoryService,
        IWindowService windowService)
    {
        _dataService = dataService;
        _settingsService = settingsService;
        _directoryService = directoryService;
        _windowService = windowService;
    }
    public ObservableCollection<Profile> Profiles { get => _dataService.Config.Profiles;}
}
