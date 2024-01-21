using SaveGameManagerMVVM.Core;
using SaveGameManagerMVVM.Interfaces;
using System.Windows.Data;
using System.Windows;
using SaveGameManagerMVVM.Models;
using System.Windows.Input;

namespace SaveGameManagerMVVM.Viewmodels
{
    public class MainViewModel : OberservableObject
    {
        private readonly IDataService _dataService;
        private ISettingsService _settingsService;
        private readonly IDirectoryService _directoryService;

        public MainViewModel(IDataService dataService, ISettingsService settingsService, IDirectoryService directoryService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _directoryService = directoryService;
            CreateSaveGameCommand = new DelegateCommand(ImportSaveGame);
            DeleteSaveGameCommand = new DelegateCommand(DeleteSaveGame);
            SelectedItemChangedCommand = new DelegateCommand(SelectedItemChanged);
        }
        public ICommand CreateSaveGameCommand { get; set; }
        public ICommand DeleteSaveGameCommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }

        private void SelectedItemChanged(object args) => SelectedSaveGame = (Savegame)args;

        public Profile SelectedProfile
        {
            get => _dataService.SelectedProfile;
            set 
            {
                if (_dataService.SelectedProfile == value)
                    return;

                _dataService.SelectedProfile = value;
                _directoryService.LoadProfile(SelectedProfile);
                OnPropertyChanged(nameof(SelectedProfile));
            }
        }

        private Savegame SelectedSaveGame { get; set; }

        public Config Config { get => _dataService.Config; }

        public ISettingsService SettingsService
        {
            get => _settingsService;
            set
            {
                if (value == _settingsService)
                    return;

                _settingsService = value;
                OnPropertyChanged(nameof(SettingsService));
            }
        }

        private void ImportSaveGame(object obj) => _directoryService.CreateSaveGame(SelectedProfile);
        private void DeleteSaveGame(object obj)
        {
            _directoryService.DeleteSaveGame(SelectedSaveGame);
            SelectedProfile.SaveGames.Remove(SelectedSaveGame);
        }


        //private void SetProfiles()
        //{
        //    var profile = cboProfile.SelectedItem as Profile;
        //    var profiles = cboProfile.ItemsSource as List<Profile>;

        //    cboProfile.ItemsSource = _profiles;

        //    if (_profiles != null && _profiles.Count > 0)
        //    {
        //        if (string.IsNullOrEmpty(_xmlHandler.ActiveProfile))
        //        {
        //            var first = _profiles.First();
        //            cboProfile.SelectedItem = first;
        //            _xmlHandler.ChangeProfile(first.Id);
        //            _directoryHandler.LoadProfile(first);
        //            tvSavegame.ItemsSource = first.SaveGames;
        //        }
        //        else
        //        {
        //            var first = _profiles.Where(p => p.Id == _xmlHandler.ActiveProfile).First();
        //            if (first != null)
        //            {
        //                if (profile == null || profile.Id != _xmlHandler.ActiveProfile)
        //                    cboProfile.SelectedItem = first;
        //            }
        //        }
        //    }
        //}

        //private void btnLoad_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(_xmlHandler.GameFolder))
        //    {
        //        MessageBox.Show("Select a gamefolder, please");
        //        return;
        //    }
        //    if (tvSavegame.SelectedItem == null)
        //    {
        //        MessageBox.Show("Select a savegame, please");
        //        return;
        //    }
        //    var savegame = tvSavegame.SelectedItem as Savegame;
        //    _directoryHandler.LoadSaveGame(savegame);

        //    MessageBox.Show($"Savefile '{savegame.Name}' loaded.");
        //}


        //private void tvSavegame_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

        //    if (treeViewItem != null)
        //    {
        //        treeViewItem.Focus();
        //        e.Handled = true;
        //    }
        //}

        //private void mtDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (tvSavegame.SelectedItem != null)
        //    {
        //        var savegame = tvSavegame.SelectedItem as Savegame;

        //        if (MessageBox.Show($"Do you really want to delete '{savegame.Name}'", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        //            return;

        //        _directoryHandler.DeleteSaveGame(savegame);

        //        var profile = cboProfile.SelectedItem as Profile;
        //        profile.SaveGames.Remove(savegame);
        //        CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource).Refresh();
        //    }
        //}

        //private void tvSavegame_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Delete)
        //    {
        //        mtDelete_Click(sender, e);
        //    }
        //}

        //private void mtRename_Click(object sender, RoutedEventArgs e)
        //{
        //    var savegame = tvSavegame.SelectedItem as Savegame;
        //    var dialog = new TextDialog(savegame.Name);

        //    if (dialog.ShowDialog() == true)
        //    {
        //        _directoryHandler.RenameSaveGameFolder(savegame, dialog.ResponseText);

        //        ICollectionView view = CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource);
        //        view.Refresh();
        //    }
        //}

        //private void mtOpenFolder_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (tvSavegame.SelectedItem != null)
        //        {
        //            var savegame = tvSavegame.SelectedItem as Savegame;
        //            if (Directory.Exists(savegame.Path))
        //                Process.Start("explorer.exe", savegame.Path);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
        //    }
        //}

        //private void btnReplace_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (tvSavegame.SelectedItem == null)
        //        {
        //            MessageBox.Show("Select a savegame, please");
        //            return;
        //        }

        //        var savegame = tvSavegame.SelectedItem as Savegame;

        //        if (MessageBox.Show($"Do you really want to replace '{savegame.Name}'", "Replace", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
        //            return;

        //        _directoryHandler.ReplaceSavegame(savegame);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
        //    }
        //}

        //private void mtDelete_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (tvSavegame.SelectedItem == null)
        //    {
        //        mtDelete.IsEnabled = false;
        //        mtOpenFolder.IsEnabled = false;
        //        mtRename.IsEnabled = false;
        //    }
        //    else
        //    {
        //        mtDelete.IsEnabled = true;
        //        mtOpenFolder.IsEnabled = true;
        //        mtRename.IsEnabled = true;
        //    }
        //}

        //private void tvSavegame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    btnLoad_Click(sender, e);
        //}

        //private void btSettings_Click(object sender, RoutedEventArgs e)
        //{
        //    cmSetting.StaysOpen = true;
        //    cmSetting.IsOpen = true;
        //}

        //private void mtAbout_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new About();
        //    dialog.ShowDialog();
        //}

        //private void mtProfiles_Click(object sender, RoutedEventArgs e)
        //{
        //    ProfileDialog profileWindow = new ProfileDialog(_xmlHandler);
        //    profileWindow.ShowDialog();

        //    SetProfiles();
        //    HandleControls(true);
        //    _directoryHandler.GameFolder = _xmlHandler.GameFolder;
        //}
    }
}
