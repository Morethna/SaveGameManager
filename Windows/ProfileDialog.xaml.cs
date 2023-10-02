using SaveGameManager.Handler;
using SaveGameManager.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SaveGameManager
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class ProfileDialog : Window
  {
    private XmlHandler _xmlHandler;
    private DirectoryHandler _directoryHandler;
    public ProfileDialog(XmlHandler xmlHandler)
    {
      InitializeComponent();
      _xmlHandler = xmlHandler;
      lvProfiles.ItemsSource = _xmlHandler.Profiles;
      txtbGamePath.Text = _xmlHandler.GameFolder;

      _directoryHandler = new DirectoryHandler(_xmlHandler.GameFolder);
      if (string.IsNullOrEmpty(txtbGamePath.Text))
      {
        btnAdd.IsEnabled = false;
        btnDelete.IsEnabled = false;
        btnEdit.IsEnabled = false;
      }
    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var dialog = new System.Windows.Forms.FolderBrowserDialog();
        dialog.InitialDirectory = @"C:\";

        if (!string.IsNullOrEmpty(txtbGamePath.Text) & Directory.Exists(txtbGamePath.Text))
          dialog.InitialDirectory = txtbGamePath.Text;


        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          txtbGamePath.Text = dialog.SelectedPath;
          btnAdd.IsEnabled = true;
          btnDelete.IsEnabled = true;
          btnEdit.IsEnabled = true;
          _xmlHandler.SetGamefolder(txtbGamePath.Text);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
      }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var dialog = new TextDialog("");
        if (dialog.ShowDialog() == true)
        {
          _xmlHandler.AddProfile(new Profile { Name = dialog.ResponseText });

          ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
          view.Refresh();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to add a profile'.\r\n{ex.Message}");
      }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (lvProfiles.SelectedItem == null)
        {
          MessageBox.Show("Select a profile, please");
          return;
        }

        var profile = lvProfiles.SelectedItem as Profile;
        var msb = $"Do you really want to delete '{profile.Name}'.{Environment.NewLine}This will also delete ALL savegames under this profile";

        if (MessageBox.Show(msb, "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
          return;

        _xmlHandler.DeleteProfile(profile);
        _directoryHandler.DeleteProfilePath(profile);

        ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
        view.Refresh();

      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to delete a profile'.\r\n{ex.Message}");
      }

    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var profile = lvProfiles.SelectedItem as Profile;

        var dialog = new TextDialog(profile.Name);
        if (dialog.ShowDialog() == true)
        {
          profile.Name = dialog.ResponseText;
          _xmlHandler.EditProfile(profile);

          ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
          view.Refresh();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to edit a profile'.\r\n{ex.Message}");
      }
    }

    private void lvProfiles_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Delete)
      {
        btnDelete_Click(sender, e);
      }
    }
  }
}
