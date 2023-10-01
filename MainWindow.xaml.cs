using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Data;
using SaveGameManager.Handler;
using SaveGameManager.Models;

namespace SaveGameManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private List<Profile> _profiles = new List<Profile>();
    private XmlHandler _xmlHandler;
    private DirectoryHandler _directoryHandler;
    public MainWindow()
    {
      InitializeComponent();
      HandleControls(false);
      _xmlHandler = new XmlHandler(_profiles);
      _xmlHandler.Init();
      _directoryHandler = new DirectoryHandler(_xmlHandler.GameFolder);

      if (!string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        SetProfiles();
        HandleControls(true);
      }
    }
    private void HandleControls(bool enable)
    {
      btnImport.IsEnabled = enable;
      btnLoad.IsEnabled = enable;
      btnReplace.IsEnabled = enable;
      tvSavegame.IsEnabled = enable;
    }

    private void SetProfiles()
    {
      cboProfile.ItemsSource = _profiles;
      if (_profiles != null && _profiles.Count > 0)
      {
        cboProfile.SelectedItem = _profiles.First();
        _directoryHandler.LoadProfile(_profiles.First());
        tvSavegame.ItemsSource = _profiles.First().SaveGames;
      }   
    }

    private void btnProfile_Click(object sender, RoutedEventArgs e)
    {
      ProfileDialog profileWindow = new ProfileDialog(_xmlHandler);
      profileWindow.ShowDialog();
    }

    private void cboProfile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void btnImport_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        MessageBox.Show("Select a gamefolder, please");
        return;
      }
      if (cboProfile.SelectedItem == null)
      {
        MessageBox.Show("Select a profile, please");
        return;
      }
      var dialog = new TextDialog($"savegame_{tvSavegame.Items.Count + 1}");
      if (dialog.ShowDialog() == true)
      {
        var profile = cboProfile.SelectedItem as Profile;
        _directoryHandler.CreateSaveGameFolder(profile, dialog.txtResponse.Text);

        tvSavegame.ItemsSource = profile.SaveGames;
      }
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        MessageBox.Show("Select a gamefolder, please");
        return;
      }
      if (tvSavegame.SelectedItem == null)
      {
        MessageBox.Show("Select a savegame, please");
        return;
      }
      var profile = tvSavegame.SelectedItem as Profile;
      _directoryHandler.LoadSaveGame(profile);
    }
  }
}
