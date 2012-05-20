using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using WhoIs.Models;

namespace WhoIs.Services
{
    public interface IUserSearchService
    {
        void BuildIndex(List<User> users);
        List<User> Search(string q);
    }

    public class UserSearchService : IUserSearchService
    {
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        public void BuildIndex(List<User> users)
        {
            FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(AppData));

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (User u in users)
            {
                indexWriter.AddDocument(u.GetDocument());
            }

            indexWriter.Optimize();
            indexWriter.Close();
        }

        public List<User> Search(string q)
        {
            FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(AppData));

            IndexReader reader = IndexReader.Open(directory, true);

            Searcher searcher = new IndexSearcher(reader);

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            var searchFields = new[] {"FirstName", "LastName", "Email", "LoginId"};
            
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, searchFields, analyzer);

            Query query = parser.Parse(q);

            TopScoreDocCollector collector = TopScoreDocCollector.create(100, true);

            searcher.Search(query, collector);

            ScoreDoc[] hits = collector.TopDocs().ScoreDocs;

            var users = new List<User>();
            
            foreach (ScoreDoc scoreDoc in hits)
            {
                //Get the document that represents the search result.
                Document document = searcher.Doc(scoreDoc.doc);

                users.Add(new User
                              {
                                  FirstName = document.Get("FirstName"),
                                  LastName = document.Get("LastName"),
                                  Email = document.Get("Email"),
                                  LoginId = document.Get("LoginId")
                              });
            }

            reader.Close();
            searcher.Close();
            analyzer.Close();

            return users;
        }
    }
}