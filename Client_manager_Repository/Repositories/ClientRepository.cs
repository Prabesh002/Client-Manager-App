using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_Manager_App_Database.AppDb;
using Client_Manager_App_Models;
using Client_manager_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Client_Manager_Repository.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDatabase _context;

        public ClientRepository(AppDatabase context)
        {
            _context = context;
        }

        public async Task<List<ClientModel>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<ClientModel> GetClientByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<List<ClientModel>> SearchClientsAsync(string searchTerm)
        {
            return await _context.Clients
                .Where(client => client.Name.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
