using Kërkues_Backend.Models;
using Microsoft.ML;

namespace Kërkues_Backend.Services
{
    public interface ISearchEngine
    {
        public SearchResult Search(string search);
    }

    public class SearchEngine : ISearchEngine
    {
        public SearchEngine()
        {
            
        }

        public SearchResult Search(string search)
        {
            var query = new Query(search);

            return query.Ranking();
        }
    }
}
