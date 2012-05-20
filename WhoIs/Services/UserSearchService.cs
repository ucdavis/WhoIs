using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using UCDArch.Core.PersistanceSupport;
using WhoIs.Models;

namespace WhoIs.Services
{
    public interface IUserSearchService
    {
        void BuildIndex(List<User> users);
    }

    public class UserSearchService : IUserSearchService
    {
        private readonly IRepositoryWithTypedId<User, Guid> _userRepository;

        public UserSearchService(IRepositoryWithTypedId<User,Guid> userRepository)
        {
            _userRepository = userRepository;
        }

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

        public List<User> Search(string field, string q)
        {
            FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(AppData));

            IndexReader reader = IndexReader.Open(directory, true);

            Searcher searcher = new IndexSearcher(reader);

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, field, analyzer);

            Query query = parser.Parse(q);

            TopScoreDocCollector collector = TopScoreDocCollector.create(100, true);

            searcher.Search(query, collector);

            ScoreDoc[] hits = collector.TopDocs().scoreDocs;

            var userIds = new List<Guid>();

            foreach (ScoreDoc scoreDoc in hits)
            {
                //Get the document that represents the search result.
                Document document = searcher.Doc(scoreDoc.doc);

                Guid id = Guid.Parse(document.Get("Id"));

                //The same document can be returned multiple times within the search results.
                if (!userIds.Contains(id))
                {
                    userIds.Add(id);
                }
            }

            //Now that we have the product Ids representing our search results, retrieve the products from the database.
            List<User> users = _userRepository.Queryable.Where(x => userIds.Contains(x.Id)).ToList();

            reader.Close();
            searcher.Close();
            analyzer.Close();

            return users;
        }
    }
}