using Client_Manager_App_Database.AppDb;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_App.Areas.Clients.Controllers
{
    [Area("Clients")]
    public class ClientController : Controller
    {
        private readonly AppDatabase _context;

        public ClientController(AppDatabase context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var clients = _context.Clients.ToList();
            return View(clients);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
