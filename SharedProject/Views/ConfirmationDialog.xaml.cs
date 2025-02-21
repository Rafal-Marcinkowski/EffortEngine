using System.Windows;

namespace SharedProject.Views;

public partial class ConfirmationDialog
{
    public string DialogText
    {
        get { return (string)GetValue(DialogTextProperty); }
        set { SetValue(DialogTextProperty, value); }
    }

    public static readonly DependencyProperty DialogTextProperty =
        DependencyProperty.Register("DialogText", typeof(string), typeof(ConfirmationDialog), new PropertyMetadata(string.Empty));

    public bool Result { get; private set; }

    public ConfirmationDialog()
    {
        InitializeComponent();
        Top = Application.Current.MainWindow.Top + 150;
        Left = Application.Current.MainWindow.Left + 174;
        FontSize = 14;
        FontWeight = FontWeights.DemiBold;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        DataContext = this;
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}
