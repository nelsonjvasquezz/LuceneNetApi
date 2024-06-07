using Lucene.Net.Util;

namespace LuceneNetApi.Services
{
    public interface IIndexService
    {
        LuceneVersion LuceneVersion { get; }

        void CreateIndex();
        bool CanCreateIt(string indexName);
    }
}