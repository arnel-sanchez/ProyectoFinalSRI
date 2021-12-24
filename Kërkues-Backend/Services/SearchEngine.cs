using Kërkues_Backend.Models;
using Microsoft.ML;
using System.Diagnostics;

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
            var timer = new Stopwatch();
            timer.Start();
            var query = new Query(search);
            var res = query.Ranking();
            timer.Stop();

            res.ResponseTime = (double)timer.ElapsedMilliseconds/(double)1000;
            return res;
        }
    }
}
