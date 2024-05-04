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
        public async Task<IActionResult> Index(string searchTerm, string filterBy, string clientType, string sortBy, string countryFilter, string genderFilter, string editingTypeFilter, bool scammerFilter, bool hasAgencyFilter, string paymentTypeFilter)
        {
            HashSet<ClientModel> hashedClients = new HashSet<ClientModel>();
            List<ClientModel> clients = new List<ClientModel>();

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

                // Check for filters and apply them
                if (!string.IsNullOrEmpty(countryFilter) || !string.IsNullOrEmpty(genderFilter) || !string.IsNullOrEmpty(editingTypeFilter) || scammerFilter || hasAgencyFilter || !string.IsNullOrEmpty(paymentTypeFilter))
                {
                    clients.Clear();
                    try
                    {
                        if (Enum.TryParse(countryFilter, out Country country))
                        {
                            var countryClients = await _clientRepository.GetClientsByCountryAsync(country);
                            if (countryClients != null)
                            {
                                hashedClients.UnionWith(countryClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No clients found for the selected country.";
                            }
                        }
                        else
                        {
                            TempData["Error"] = $"Invalid country filter: {countryFilter}";
                        }

                        if (Enum.TryParse(genderFilter, out Gender gender))
                        {
                            var genderClients = await _clientRepository.GetClientsByGenderAsync(gender);
                            if (genderClients != null)
                            {
                                hashedClients.UnionWith(genderClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No clients found for the selected gender.";
                            }
                        }
                        else
                        {
                            TempData["Error"] = $"Invalid gender filter: {genderFilter}";
                        }

                        if (!string.IsNullOrEmpty(editingTypeFilter))
                        {
                            var editingTypeClients = await _clientRepository.GetClientsByEditingTypeAsync(editingTypeFilter);
                            if (editingTypeClients != null)
                            {
                                hashedClients.UnionWith(editingTypeClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No clients found for the selected editing type.";
                            }
                        }

                        if (scammerFilter)
                        {
                            var scammerClients = await _clientRepository.GetScammerClientsAsync();
                            if (scammerClients != null)
                            {
                                hashedClients.UnionWith(scammerClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No scammer clients found.";
                            }
                        }

                        if (hasAgencyFilter)
                        {
                            var agencyClients = await _clientRepository.GetClientsWithAgencyAsync();
                            if (agencyClients != null)
                            {
                                hashedClients.UnionWith(agencyClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No clients with agency found.";
                            }
                        }

                        if (!string.IsNullOrEmpty(paymentTypeFilter))
                        {
                            var paymentTypeClients = await _clientRepository.GetClientsByPaymentTypeAsync(paymentTypeFilter);
                            if (paymentTypeClients != null)
                            {
                                hashedClients.UnionWith(paymentTypeClients);
                            }
                            else
                            {
                                TempData["Error"] = $"No clients found for the selected payment type.";
                            }
                        }

                        clients.AddRange(hashedClients); // Convert HashSet to List
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                        TempData["Error"] = $"An error occurred while applying filters: {ex.Message}";
                    }
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
