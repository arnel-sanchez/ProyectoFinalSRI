namespace Kërkues_Backend.Models
{
    public class SearchResult
    {
        public double ResponseTime { get; set; }

        public List<SearchSuggestion> SearchSuggestion { get; set; }

        public List<SearchObjectResult> SearchObjectResults { get; set; }
    }
}
