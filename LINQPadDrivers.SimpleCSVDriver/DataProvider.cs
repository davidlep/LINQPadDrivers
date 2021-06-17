using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public class DataProvider<TRecord> : IDataProvider<TRecord>
    {
		public static IEnumerable<TRecord> GetRecordsCsv(string filepath)
		{
            using var sr = new StreamReader(filepath);
            using var reader = new CsvReader(sr, CultureInfo.InvariantCulture);

            reader.Configuration.BadDataFound = context => { };

            foreach (var record in reader.GetRecords<TRecord>())
                yield return record;
        }

        public IEnumerable<TRecord> GetRecordsFromText(string text)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TRecord> GetRecordsFromFile(string filepath)
        {
            using var sr = new StreamReader(filepath);





            throw new NotImplementedException();
        }

        public IEnumerable<TRecord> GetRecordsFromFolder(string folderpath)
        {
            //Une approche possible, mais probablement pas très bon pour des très gros fichier car tout est loadé en mémoire:
            //---------------------

            //var messages = new List<FacebookMessageModel>();
            //foreach (var file in Directory.GetFiles(directory))
            //{
            //    var messagesPart = File.ReadAllText(file);
            //    var messageModels = (JsonConvert.DeserializeObject<dynamic>(messagesPart).messages as JArray).ToObject<FacebookMessage[]>().Select(ToModel);
            //    messages.AddRange(messageModels);
            //}
            //return messages.ToArray();

            //Peut-être une approche de lazy loadé facile à faire avec un enumerator
            //---------------------

            throw new NotImplementedException();
        }

        public IEnumerable<TRecord> GetRecordsFromURL(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
