using SaveGameManager.Handler;
using SaveGameManager.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SaveGameManager
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class ProfileDialog : Window
  {
    private XmlHandler _xmlHandler;
    public ProfileDialog(XmlHandler xmlHandler)
    {
      InitializeComponent();
      _xmlHandler = xmlHandler;
      lvProfiles.ItemsSource = _xmlHandler.Profiles;
      txtbGamePath.Text = _xmlHandler.GameFolder;

      if (string.IsNullOrEmpty(txtbGamePath.Text))
      {
        btnAdd.IsEnabled = false;
        btnDelete.IsEnabled = false;
        btnEdit.IsEnabled = false;
      }
    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();
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

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new TextDialog("");
      if (dialog.ShowDialog() == true)
      {
        _xmlHandler.AddProfile(new Profile { Name = dialog.ResponseText });
        lvProfiles.ItemsSource = _xmlHandler.Profiles;
        ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
        view.Refresh();
      }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (lvProfiles.SelectedItem == null)
        System.Windows.MessageBox.Show("Select a profile, please");

      _xmlHandler.DeleteProfile((Profile)lvProfiles.SelectedItem);
      lvProfiles.ItemsSource = _xmlHandler.Profiles;
      ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
      view.Refresh();
    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
      if (lvProfiles.SelectedItem == null)
        System.Windows.MessageBox.Show("Select a profile, please");

      Profile profile = (Profile)lvProfiles.SelectedItem;
      var dialog = new TextDialog(profile.Name);
      if (dialog.ShowDialog() == true)
      {
        profile.Name = dialog.ResponseText;
        _xmlHandler.EditProfile(profile);
        lvProfiles.ItemsSource = _xmlHandler.Profiles;
        ICollectionView view = CollectionViewSource.GetDefaultView(lvProfiles.ItemsSource);
        view.Refresh();
      }

    }
  }
}
