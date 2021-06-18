using Davidlep.LINQPadDrivers.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Davidlep.LINQPadDrivers.SimpleJsonDriver
{
    public class JsonDataProvider<TRecord> : BaseDataProvider<TRecord>
	{
		public override IEnumerable<TRecord> GetRecords(StreamReader sr)
		{
			using var reader = new JsonTextReader(sr);

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
