<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

var currentPath = Path.GetDirectoryName(Util.CurrentQueryPath);
var outputExt = "lpx6";

//Config
var outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var buildPath = @"\bin\Release\netcoreapp3.0";
var projectPaths = new[]
{
	currentPath + @"\LINQPadDrivers.SimpleCSVDriver",
	currentPath + @"\LINQPadDrivers.SimpleJsonDriver",
};

var projects = projectPaths
	.Select(x => Directory.GetFiles(x, "*.csproj"))
	.SelectMany(x => x)
	.Select(x => new FileInfo(x))
	.Select(x => new { FileInfo = x, Csproj = GetCsproj(x.FullName)});


//Package
projects
	.Select(x => new { AssemblyName = x.Csproj.Descendants().First(x => x.Name == "AssemblyName").Value, x.FileInfo})
	.Select(x => new {SourceDirectoryName =$"{x.FileInfo.Directory}{buildPath}", DestinationArchiveFileName = $"{outputFolder}\\{x.AssemblyName}.{outputExt}"})
	.Dump()
	.ToList()
	.ForEach(x => ZipFile.CreateFromDirectory(x.SourceDirectoryName, x.DestinationArchiveFileName));

XDocument GetCsproj(string csprojFilePath)
{
	var xmlData = File.ReadAllText(csprojFilePath);
	return XDocument.Parse(xmlData);
}