﻿using System.Windows;

namespace SaveGameManager.Views;

/// <summary>
/// Interaction logic for MessageBox.xaml
/// </summary>
public partial class NotificationBox : Window
{
    public NotificationBox()
    {
        InitializeComponent();
    }

    private void wdDialog_GotFocus(object sender, RoutedEventArgs e) => btnNo.Focus();
}
