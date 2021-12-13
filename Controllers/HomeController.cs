using Microsoft.AspNetCore.Mvc;
using ProyectoFinalSRI.Services;

namespace ProyectoFinalSRI.Controllers
{
    public class HomeController : Controller
    {
        private ISearchEngine SearchEngine;

        public HomeController(ISearchEngine searchEngine)
        {
            SearchEngine = searchEngine;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Post(string search="")
        {
            return View("Results", SearchEngine.Search(search));
        }
    }
}
