﻿namespace Kërkues_Backend.Models
{
    public class SearchResult
    {
        public List<SearchSuggestion> SearchSuggestion { get; set; }

        public List<SearchObjectResult> SearchObjectResults { get; set; }
    }
}
