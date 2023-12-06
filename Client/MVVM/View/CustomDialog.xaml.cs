using System.Windows;
using System.Windows.Input;

namespace Client.MVVM.View;

public partial class CustomDialog : Window
{
    public string TitleText { get; set; }
    public string MessageText { get; set; }

    public CustomDialog(string title, string message)
    {
        InitializeComponent();
        TitleText = title;
        MessageText = message;
        DataContext = this;
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
    private void HeaderPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
}