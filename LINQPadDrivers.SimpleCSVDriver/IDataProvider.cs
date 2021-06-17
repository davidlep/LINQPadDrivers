using System;
using System.Collections.Generic;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public interface IDataProvider<TRecord>
    {
        IEnumerable<TRecord> GetRecordsFromFile(string filepath);
        IEnumerable<TRecord> GetRecordsFromFolder(string folderpath);
        IEnumerable<TRecord> GetRecordsFromText(string text); //Pour le Clipboard
        IEnumerable<TRecord> GetRecordsFromURL(Uri uri);
    }
}