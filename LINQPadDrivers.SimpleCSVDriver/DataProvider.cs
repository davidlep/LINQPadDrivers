using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public static class DataProvider
    {
		public static IEnumerable<TRecord> GetRecordsCsv<TRecord>(string filepath)
		{
            using var sr = new StreamReader(filepath);
            using var reader = new CsvReader(sr, CultureInfo.InvariantCulture);

            reader.Configuration.BadDataFound = context => { };

            foreach (var record in reader.GetRecords<TRecord>())
                yield return record;
        }
	}
}
