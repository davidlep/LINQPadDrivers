using LINQPad.Extensibility.DataContext;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    class ConnectionProperties : INotifyPropertyChanged
	{
		public IConnectionInfo ConnectionInfo { get; private set; }

		XElement DriverData => ConnectionInfo.DriverData;

		public ConnectionProperties(IConnectionInfo connectionInfo)
		{
			ConnectionInfo = connectionInfo;

			if (!connectionInfo.DriverData.Elements("UseTypeInference").Any())
				this.UseTypeInference = true;
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

		public bool UseTypeInference
		{
			get => (bool)DriverData.Element(nameof(UseTypeInference));
			set
			{
				DriverData.SetElementValue(nameof(UseTypeInference), value);
				OnPropertyChanged(nameof(UseTypeInference));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}