using Kërkues_Backend.Models;

namespace ProyectoFinalSRI.Services
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
            throw new NotImplementedException();
        }
    }
}
