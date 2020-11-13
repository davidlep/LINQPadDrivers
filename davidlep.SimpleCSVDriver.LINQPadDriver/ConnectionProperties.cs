using LINQPad.Extensibility.DataContext;
using System.ComponentModel;
using System.Xml.Linq;

namespace davidlep.SimpleCSVDriver.LINQPadDriver
{
    class ConnectionProperties : INotifyPropertyChanged
	{
		public IConnectionInfo ConnectionInfo { get; private set; }

		XElement DriverData => ConnectionInfo.DriverData;

		public ConnectionProperties(IConnectionInfo connectionInfo)
		{
			ConnectionInfo = connectionInfo;
		}

		public string FilePath
		{
			get => (string)DriverData.Element(nameof(FilePath));
			set
			{
				DriverData.SetElementValue(nameof(FilePath), value);
				OnPropertyChanged(nameof(FilePath));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}