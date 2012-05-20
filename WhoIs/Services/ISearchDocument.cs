using Lucene.Net.Documents;

namespace WhoIs.Services
{
    public interface ISearchDocument
    {
        Document GetDocument();
    }
}