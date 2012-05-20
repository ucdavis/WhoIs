using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace WhoIs.Services
{
    public interface ISearchService
    {
        void BuildIndex<T>(List<T> products) where T : ISearchDocument;
    }

    public class SearchService : ISearchService
    {
            public void BuildIndex<T>(List<T> products) where T : ISearchDocument
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(appdata));

                Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

                IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

                foreach (T product in products)
                {
                    indexWriter.AddDocument(product.GetDocument());
                }

                indexWriter.Optimize();
                indexWriter.Close();
            }
    }
}