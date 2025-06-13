using System.Windows;
using System.Windows.Controls;

namespace SwampDbg.Controls
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl
    {
        public TestControl()
        {
            InitializeComponent();
        }

		private void OnAddTop(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnAddTop");
		}

		private void OnRemoveTop(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnRemoveTop");
		}

		private void OnAddBottom(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnAddBottom");
		}

		private void OnRemoveBottom(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnRemoveBottom");
		}

		private void OnAddLeft(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnAddLeft");
		}

		private void OnRemoveLeft(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnRemoveLeft");
		}

		private void OnAddRight(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnAddRight");
		}

		private void OnRemoveRight(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnRemoveRight");
		}

		private void OnAddCenter(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnAddCenter");
		}

		private void OnRemoveCenter(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnRemoveCenter");
		}
	}
}
