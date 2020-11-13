using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using LINQPad.Extensibility.DataContext;
using Microsoft.Win32;

namespace davidlep.SimpleCSVDriver.LINQPadDriver
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
				Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				(DataContext as ConnectionProperties).FilePath = openFileDialog.FileName;
			}
		}
	}
}