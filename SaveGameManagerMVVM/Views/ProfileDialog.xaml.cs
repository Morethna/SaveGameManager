﻿using System.Windows;

namespace SaveGameManagerMVVM.Views;
/// <summary>
/// Interaction logic for Profile.xaml
/// </summary>
public partial class ProfileDialog : Window
{
    public ProfileDialog()
    {
        InitializeComponent();
    }

    private void lvProfiles_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {

    }
}