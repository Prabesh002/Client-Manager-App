using Client_Manager_App.Models;
using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
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


      

        public async  Task<IActionResult> Index(string searchTerm, string filterBy, string clientType)
        {
            List<ClientModel> clients;

            // Check if client type filter is selected
            if (!string.IsNullOrEmpty(clientType))
            {
                // Convert client type string to enum
                if (Enum.TryParse(clientType, out ClientType type))
                {
                    // Filter clients by client type
                    clients = await _clientRepository.GetClientsByTypeAsync(type);
                }
                else
                {
                    // Handle invalid client type
                    clients = new List<ClientModel>();
                }
            }
            else
            {

                clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
            }

            return View(clients);

        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
