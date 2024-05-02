using Client_Manager_App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Client_Manager_App_Models;
using Client_Manager_App.AppDb;

namespace Client_Manager_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly AppDatabase _context;

        public HomeController(AppDatabase context)
        {
            _context = context;
        }

        public IActionResult GetClientDetails(int id)
        {
            var client = _context.Clients.Find(id);
            return PartialView("_ClientDetailsPartial", client);
        }


      

        public IActionResult Index()
        {
            var clients = _context.Clients.ToList();
            return View(clients);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
