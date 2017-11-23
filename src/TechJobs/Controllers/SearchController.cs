using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TechJobs.Models;

namespace TechJobs.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.columns = ListController.columnChoices;
            ViewBag.title = "Search";
            return View();
        }

        // TODO #1 - Create a Results action method to process 
        // search request and display results

        public IActionResult Results(string searchType, string searchTerm)
        {
            ViewBag.columns = ListController.columnChoices;
            ViewBag.title = "Search Results";
            List<Dictionary<string, string>> searchResults;

            if (searchType.ToLower().Equals("all"))
            {
                searchResults = JobData.FindByValue(searchTerm);
                ViewBag.searchResults = searchResults;
            }
            else
            {
                searchResults = JobData.FindByColumnAndValue(searchType, searchTerm);
                ViewBag.searchResults = searchResults;
            }

            return View("Index");
        }
    }
}
