using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
using Client_manager_Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (file == null || file.Length == 0)
            {
                TempData["Success"] = "Please select a file to upload.";
                return RedirectToAction("Client");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                       


                        if (worksheet == null)
                        {
                            TempData["Success"] = "No worksheet found in the uploaded Excel file.";
                            return RedirectToAction("Client");
                        }

                        int rows = worksheet.Dimension.Rows;
                        int columns = worksheet.Dimension.Columns;

                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            for (int row = 2; row <= rows; row++) // the first row contains headers
                            {
                                ClientModel client = new ClientModel();
                                bool isValid = true;

                                for (int col = 1; col <= columns; col++)
                                {
                                    string value = worksheet.Cells[row, col].Value?.ToString();
                                    if (value != null)
                                    {
                                        switch (col)
                                        {
                                            case 1: 
                                                client.Name = value;
                                                break;
                                            case 2: 
                                                client.Email = value;
                                                break;
                                            case 3: 
                                                client.PlatformUrl = value;
                                                break;
                                            case 4:
                                                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                                                {
                                                    client.TimeEmailSent = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 5:
                                                
                                                if (string.IsNullOrWhiteSpace(value)) 
                                                {
                                                    client.IsRejected = false;
                                                }
                                                else if (bool.TryParse(value, out bool reject))
                                                {
                                                    client.IsRejected = reject;
                                                }
                                                else 
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 6: 
                                                client.MaxOffer = value;
                                                break;
                                            case 7: 
                                                if (Enum.TryParse(value, out ClientType clientType))
                                                {
                                                    client.ClientType = clientType;
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 8: 
                                                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastUpdated))
                                                {
                                                    client.LastUpdated = DateTime.SpecifyKind(lastUpdated, DateTimeKind.Utc);
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 9: 
                                                client.ReWorkingRate = value;
                                                break;
                                            case 10: 
                                                if (Enum.TryParse(value, out Gender gender))
                                                {
                                                    client.Gender = gender;
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 11: 
                                                if (Enum.TryParse(value, out Country country))
                                                {
                                                    client.Country = country;
                                                }
                                                else
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 12: 
                                                client.EditingType = value;
                                                break;
                                            case 13:
                                                if (string.IsNullOrWhiteSpace(value)) // If value is empty or whitespace, default to false
                                                {
                                                    client.IsScammer = false;
                                                }
                                                else if (bool.TryParse(value, out bool isScammer)) // Otherwise, try parsing the value
                                                {
                                                    client.IsScammer = isScammer;
                                                }
                                                else // Parsing failed
                                                {
                                                    isValid = false;
                                                }
                                                break;
                                            case 14:
                                                if (string.IsNullOrWhiteSpace(value)) // If value is empty or whitespace, default to false
                                                {
                                                    client.HasAgency = false;
                                                }
                                                else if (bool.TryParse(value, out bool hasAgency)) // Otherwise, try parsing the value
                                                {
                                                    client.HasAgency = hasAgency;
                                                }
                                                else // Parsing failed
                                                {
                                                    isValid = false;
                                                }
                                                break;

                                            case 15: 
                                                client.PaymentType = value;
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                    else if (col == 1 || col == 2) // Check for required fields
                                    {
                                        isValid = false;
                                    }
                                }

                                if (isValid)
                                {
                                    _context.Clients.Add(client);
                                }
                                else
                                {
                                    TempData["Success"] = $"Error in row {row}: Required fields are missing or date format is invalid.";
                                    return RedirectToAction("Client");
                                }
                            }

                            await _context.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                }

                TempData["Success"] = "Data uploaded successfully!";
                return RedirectToAction("Client");
            }
            catch (Exception ex)
            {
                TempData["Success"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Client");
            }


        }
    }
}
