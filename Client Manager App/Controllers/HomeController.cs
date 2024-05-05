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
            List<ClientModel> clients = new List<ClientModel>();
            var query = await _clientRepository.GetAllClientsQueryableAsync();
            if (!string.IsNullOrEmpty(clientType))
            {
                if (Enum.TryParse(clientType, out ClientType type))
                {
                    clients = await _clientRepository.GetClientsByTypeAsync(type);
                }
                else
                {
                    // invalid client type
                    clients = new List<ClientModel>();
                }
            }
            else if (searchTerm != null && filterBy != null)
            {
                clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
            }
            else
            {
                switch (sortBy)
                {
                    case "timeEmailSentDesc":
                        query = query.OrderByDescending(client => client.TimeEmailSent);
                        break;
                    case "timeEmailSentAsc":
                        query = query.OrderBy(client => client.TimeEmailSent);
                        break;
                    case "maxOfferAsc":
                        query = query.OrderBy(client => client.MaxOffer);
                        break;
                    case "maxOfferDesc":
                        query = query.OrderByDescending(client => client.MaxOffer);
                        break;
                    case "newestUpdated":
                        query = query.OrderByDescending(client => client.LastUpdated);
                        break;
                    case "oldestUpdated":
                        query = query.OrderBy(client => client.LastUpdated);
                        break;
                }



                try
                {
                    if (!string.IsNullOrEmpty(countryFilter))
                    {
                        if (Enum.TryParse(countryFilter, out Country country))
                        {
                            query = query.Where(c => c.Country == country);
                        }
                        else
                        {
                            TempData["Error"] = $"Invalid country filter: {countryFilter}";
                            return View(new List<ClientModel>());
                        }
                    }

                    if (!string.IsNullOrEmpty(genderFilter))
                    {
                        if (Enum.TryParse(genderFilter, out Gender gender))
                        {
                            query = query.Where(c => c.Gender == gender);
                        }
                        else
                        {
                            TempData["Error"] = $"Invalid gender filter: {genderFilter}";
                            return View(new List<ClientModel>());
                        }
                    }

                    if (!string.IsNullOrEmpty(editingTypeFilter))
                    {
                        query = query.Where(c => c.EditingType == editingTypeFilter);
                    }

                    if (scammerFilter)
                    {
                        query = query.Where(c => c.IsScammer);
                    }

                    if (hasAgencyFilter)
                    {
                        query = query.Where(c => c.HasAgency);
                    }

                    if (!string.IsNullOrEmpty(paymentTypeFilter))
                    {
                        query = query.Where(c => c.PaymentType == paymentTypeFilter);
                    }

                    clients = query.ToList();


                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    TempData["Error"] = $"An error occurred while applying filters: {ex.Message}";
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
