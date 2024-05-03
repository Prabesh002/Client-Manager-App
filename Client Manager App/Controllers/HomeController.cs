using Client_Manager_App.Models;
using Client_Manager_App_Database.AppDb;
using Client_manager_Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client_Manager_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly AppDatabase _context;
        private readonly IClientRepository _clientRepository;

        public HomeController(AppDatabase context, IClientRepository clientRepository)
        {
            _context = context;
            _clientRepository = clientRepository;
        }

        public IActionResult GetClientDetails(int id)
        {
            var client = _context.Clients.Find(id);
            return PartialView("_ClientDetailsPartial", client);
        }


      

        public IActionResult Index(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var clients = _clientRepository.SearchClientsAsync(searchTerm).Result;
                return View(clients);
            }
            else
            {
                var clients = _context.Clients.ToList();
                return View(clients);
            }
            
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
