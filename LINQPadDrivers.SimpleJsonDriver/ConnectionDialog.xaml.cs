using System.Windows;

using LINQPad.Extensibility.DataContext;
using Microsoft.Win32;

namespace Davidlep.LINQPadDrivers.SimpleJsonDriver
{
    public partial class ConnectionDialog : Window
	{
		readonly IConnectionInfo connectionInfo;

		public ConnectionDialog(IConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
			DataContext = new ConnectionProperties(connectionInfo);
			InitializeComponent();
		}

		void BtnOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Browser_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "Json files (*.json)|*.json|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				(DataContext as ConnectionProperties).FilePath = openFileDialog.FileName;
			}
		}
	}
}