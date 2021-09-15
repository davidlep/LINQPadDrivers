using System.Windows;

using LINQPad.Extensibility.DataContext;
using Microsoft.Win32;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public partial class DataSourcesConnectionDialog : Window
	{
		readonly IConnectionInfo connectionInfo;

		public DataSourcesConnectionDialog(IConnectionInfo connectionInfo)
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
				Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				(DataContext as ConnectionProperties).FilePath = openFileDialog.FileName;
			}
		}
	}
}