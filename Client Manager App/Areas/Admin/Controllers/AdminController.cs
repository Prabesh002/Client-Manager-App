using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
using Client_manager_Repository.Interfaces;
using Ganss.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Globalization;
using System.Reflection;

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

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            using (var stream = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var mapper = new ExcelMapper();

                    string tempFilePath = Path.GetTempFileName();
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                    var clientModels = mapper.Fetch<ClientModel>(tempFilePath).ToList();

                    System.IO.File.Delete(tempFilePath);
                    _context.Clients.AddRange(clientModels);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sucessfully added all the entries";
                }
                catch (Exception ex) 
                {
                    TempData["Success"] = $" Error! \n  {ex.Message}";
                }
            }

            return Ok();
        }



    }
}
