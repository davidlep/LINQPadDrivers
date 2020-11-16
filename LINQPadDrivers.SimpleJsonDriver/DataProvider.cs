using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Davidlep.LINQPadDrivers.SimpleJsonDriver
{
    public static class DataProvider
    {
		public static IEnumerable<TRecord> GetRecordsJson<TRecord>(string filepath)
		{
			using (var sr = new StreamReader(filepath))
			using (var reader = new JsonTextReader(sr))
			{
				reader.SupportMultipleContent = true;

				var serializer = new JsonSerializer();
				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.StartObject)
					{
						yield return serializer.Deserialize<TRecord>(reader);
					}
				}
			}
		}
	}
}
