using CsvHelper;
using Davidlep.LINQPadDrivers.Common;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public abstract class CsvDataProvider<TRecord> : BaseDataProvider<TRecord>
    {
        public override IEnumerable<TRecord> GetRecords(StreamReader sr)
        {
            using var reader = new CsvReader(sr, CultureInfo.InvariantCulture);

            reader.Configuration.BadDataFound = context => { };

            foreach (var record in reader.GetRecords<TRecord>())
                yield return record;
        }
    }
}
