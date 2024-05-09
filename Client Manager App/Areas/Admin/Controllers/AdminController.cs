using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
using Client_manager_Repository.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Globalization;
using System.Reflection;
using System.IO;
namespace Client_Manager_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDatabase _context;
        private readonly IClientRepository _clientRepository;
        public AdminController(AppDatabase context, IClientRepository clientRepository)
        {
            _context = context;
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Client(string searchTerm, string filterBy, string clientType)
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
                    clients = new List<ClientModel>();
                }
            }
            else
            {

                clients = await _clientRepository.GetFilteredClientsAsync(searchTerm, filterBy);
            }

            return View(clients);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClientModel client)
        {
            if (ModelState.IsValid)
            {
                client.TimeEmailSent = client.TimeEmailSent.ToUniversalTime();
                client.LastUpdated = client.LastUpdated.ToUniversalTime();
                _context.Clients.Add(client);
                _context.SaveChanges();
                TempData["Success"] = $"{client.Name} was added successfully!";
                return RedirectToAction("Client");
            }
            return View(client);
        }

        public IActionResult Edit(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        [HttpPost]
        public IActionResult Edit(int id, ClientModel client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                client.TimeEmailSent = client.TimeEmailSent.ToUniversalTime();
                client.LastUpdated = client.LastUpdated.ToUniversalTime();
                _context.Update(client);
                _context.SaveChanges();
                TempData["Success"] = "Data Updated sucessfully";
                return RedirectToAction("Client");
            }
            return View(client);
        }

        public IActionResult Delete(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();

            TempData["Success"] = $"{client.Name} was removed";
            return RedirectToAction("Client");
        }

        public IActionResult OpenDelete()
        {
            var client = _context.Clients.ToList();
            return PartialView("_DeletePartial", client);
        }

        [HttpPost]
        public IActionResult DeleteMultiple(List<int> selectedClients)
        {
            if (selectedClients == null || selectedClients.Count == 0)
            {
                TempData["Error"] = "No clients selected for deletion.";
                return RedirectToAction("Client");
            }

            foreach (var clientId in selectedClients)
            {
                var client = _context.Clients.Find(clientId);
                if (client != null)
                {
                    _context.Clients.Remove(client);
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Selected clients were removed";
            return RedirectToAction("Client");
        }


        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            try
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\folders";
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var clients = new List<ClientModel>();

                            while (reader.Read())
                            {
                                ClientModel cm = new ClientModel();
                                cm.Name = reader.GetValue(0)?.ToString() ?? "N/D";
                                cm.Email = reader.GetValue(1)?.ToString() ?? "N/D";
                                cm.PlatformUrl = reader.GetValue(2)?.ToString() ?? string.Empty;
                                cm.MaxOffer = reader.GetValue(5)?.ToString() ?? "N/D";
                                cm.ReWorkingRate = reader.GetValue(8)?.ToString() ?? "N/D";
                                cm.EditingType = reader.GetValue(11)?.ToString() ?? "N/D";
                                cm.PaymentType = reader.GetValue(14)?.ToString() ?? "N/D";

                                bool.TryParse(reader.GetValue(4)?.ToString(), out var isRejected);
                                cm.IsRejected = isRejected;

                                bool.TryParse(reader.GetValue(12)?.ToString(), out var isScammer);
                                cm.IsScammer = isScammer;

                                bool.TryParse(reader.GetValue(13)?.ToString(), out var hasAgency);
                                cm.HasAgency = hasAgency;

                                Enum.TryParse<ClientType>(reader.GetValue(6)?.ToString(), out var clientType);
                                cm.ClientType = clientType != null ? clientType : ClientType.empty;

                                Enum.TryParse<Gender>(reader.GetValue(9)?.ToString(), out var gender);
                                cm.Gender = gender != null ? gender : Gender.NotDisclosed;

                                Enum.TryParse<Country>(reader.GetValue(10)?.ToString(), out var country);
                                cm.Country = country != null ? country : Country.USA;

                                DateTime.TryParse(reader.GetValue(3)?.ToString(), out var timeEmailSent);
                                cm.TimeEmailSent = timeEmailSent != DateTime.MinValue ? timeEmailSent : DateTime.Now;

                                DateTime.TryParse(reader.GetValue(7)?.ToString(), out var lastUpdated);
                                cm.LastUpdated = lastUpdated != DateTime.MinValue ? lastUpdated : DateTime.Now;
                                cm.TimeEmailSent =  cm.TimeEmailSent.ToUniversalTime();
                                cm.LastUpdated = cm.LastUpdated.ToUniversalTime();
                                clients.Add(cm);
                            }
                            _context.Clients.AddRange(clients);
                            _context.SaveChanges();

                            TempData["Success"] = "File uploaded successfully.";
                        }
                    }
                }
                else
                {
                    TempData["Success"] = "No file selected.";
                }
            }
            catch (Exception ex)
            {
                TempData["Success"] = $"Error uploading file: {ex.Message}";
            }

            return RedirectToAction("Client");
        }

    }
}
