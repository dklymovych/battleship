using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Client.MVVM.View;

public partial class ShipSelectionView : Window
{
    public int SelectedLength { get; private set; }
    public string SelectedOrientation { get; private set; } = "Horizontal";
    private Dictionary<int, int> _shipLengths;
    

    public ShipSelectionView(Dictionary<int, int> shipLengths)
    {
        InitializeComponent();
        _shipLengths = shipLengths;
        UpdateButtonAvailability();
    }
    private void UpdateButtonAvailability()
    {
        // Assuming you have buttons named length1Button, length2Button, etc.
        length1Button.Content = $"1 ({_shipLengths[1]})";
        length2Button.Content = $"2 ({_shipLengths[2]})";
        length3Button.Content = $"3 ({_shipLengths[3]})";
        length4Button.Content = $"4 ({_shipLengths[4]})";

        length1Button.IsEnabled = _shipLengths[1] > 0;
        length2Button.IsEnabled = _shipLengths[2] > 0;
        length3Button.IsEnabled = _shipLengths[3] > 0;
        length4Button.IsEnabled = _shipLengths[4] > 0;
    }

    private void LengthButton_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = sender as Button;
        SelectedLength = int.Parse(clickedButton.Tag.ToString());
        // Add visual feedback here if needed
    }

    private void ChangeOrientationButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedOrientation == "Horizontal")
        {
            SelectedOrientation = "Vertical";
            currentOrientationText.Content = "Vertical";
        }
        else
        {
            SelectedOrientation = "Horizontal";
            currentOrientationText.Content = "Horizontal";
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedLength == 0)
        {
            alertTextBlock.Visibility = Visibility.Visible; // Show the alert message
            return;
        }

        this.DialogResult = true;
    }


    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}