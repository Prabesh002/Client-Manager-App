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



        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, string filterBy, string clientType, string sortBy)
        {
            List<ClientModel> clients;

            if (!string.IsNullOrEmpty(clientType))
            {
               
                if (Enum.TryParse(clientType, out ClientType type))
                {
                   
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
                // Apply sorting if specified
                switch (sortBy)
                {
                    case "timeEmailSentDesc":
                        clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
                        clients = clients.OrderByDescending(c => c.TimeEmailSent).ToList();
                        break;
                    case "timeEmailSentAsc":
                        clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
                        clients = clients.OrderBy(c => c.TimeEmailSent).ToList();
                        break;
                    case "maxOfferAsc":
                        clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
                        clients = clients.OrderBy(c => c.MaxOffer).ToList();
                        break;
                    case "maxOfferDesc":
                        clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
                        clients = clients.OrderByDescending(c => c.MaxOffer).ToList();
                        break;
                    case "newestUpdated":
                        clients = await _clientRepository.GetClientsSortedByNewestUpdatedAsync();
                        break;
                    case "oldestUpdated":
                        clients = await _clientRepository.GetClientsSortedByOldestUpdatedAsync();
                        break;
                    default:
                     
                        clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
                        break;
                }
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
