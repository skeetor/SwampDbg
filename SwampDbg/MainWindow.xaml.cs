namespace SwampDbg;

using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

	private void OnOpenFile(object sender, RoutedEventArgs e)
	{
		MessageBox.Show("OnOpenFile");
	}

	private void OnOpenProject(object sender, RoutedEventArgs e)
	{
		MessageBox.Show("OnOpenProject");
	}
}
