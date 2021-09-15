using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Davidlep.LINQPadDrivers.Common
{
    public abstract class BaseDataProvider<TRecord>
    {
        public abstract IEnumerable<TRecord> GetRecords(StreamReader sr);

        public IEnumerable<TRecord> GetRecordsFromText(string text)
        {
            using var s = StreamFromString(text);
            using var sr = new StreamReader(s);
            
            return GetRecords(sr);
        }

        public IEnumerable<TRecord> GetRecordsFromFile(string filepath)
        {
            using var sr = new StreamReader(filepath);
            
            return GetRecords(sr);
        }

        public IEnumerable<TRecord> GetRecordsFromFolder(string folderpath, string searchPattern)
        {
            return Directory.GetFiles(folderpath, searchPattern)
                .Select(GetRecordsFromFile)
                .SelectMany(x => x);
        }

        public IEnumerable<TRecord> GetRecordsFromURL(string requestUri)
        {
            var httpClient = new HttpClient();
            using var s = Task.Run(() => httpClient.GetStreamAsync(requestUri)).Result;
            using var sr = new StreamReader(s);
            
            return GetRecords(sr);
        }

        private Stream StreamFromString(string text)
        {
            var stream = new MemoryStream() { Position = 0 };
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            return stream;
        }
    }
}
