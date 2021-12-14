using Kërkues_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalSRI.Services;

namespace Kërkues_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchEngine _searchEngine;

        public SearchController(ISearchEngine searchEngine)
        {
            _searchEngine = searchEngine;
        }

        [HttpPost]
        public IActionResult Post(SearchRequest request)
        {
            return Ok(_searchEngine.Search(request.Search));
        }
    }
}
