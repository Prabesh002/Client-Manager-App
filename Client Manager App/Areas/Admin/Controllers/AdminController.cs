using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
using Client_manager_Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDatabase _context;
        private readonly IClientRepository _clientRepository;
        public AdminController(AppDatabase context , IClientRepository clientRepository)
        {
            _context = context;
            _clientRepository = clientRepository;
        }

        public IActionResult Client(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var clients = _clientRepository.SearchClientsAsync(searchTerm).Result; 
                return View(clients);
            }
            else
            {
                var clients = _clientRepository.GetAllClientsAsync().Result;
                return View(clients);
            }
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


    }
}
