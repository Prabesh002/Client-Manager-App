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

        public async Task<List<ClientModel>> GetClientsByTypeAsync(ClientType type)
        {
            return await _context.Clients.Where(c => c.ClientType == type).ToListAsync();
        }

        public async Task<List<ClientModel>> GetFilteredClientsAsync(string searchTerm, string filterBy)
        {
            IQueryable<ClientModel> query = _context.Clients;

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "Name":
                        query = query.Where(c => c.Name.Contains(searchTerm));
                        break;
                    case "Email":
                        query = query.Where(c => c.Email.Contains(searchTerm));
                        break;
                    case "MaxOffer":
                        query = query.Where(c => c.MaxOffer.Contains(searchTerm));
                        break;
                    default:
                        break;
                }
            }

            return await query.ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByTimeEmailSentDescAsync()
        {
            return await _context.Clients.OrderByDescending(client => client.TimeEmailSent).ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByTimeEmailSentAscAsync()
        {
            return await _context.Clients.OrderBy(client => client.TimeEmailSent).ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByMaxOfferAscAsync()
        {
            return await _context.Clients.OrderBy(client => client.MaxOffer).ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByMaxOfferDescAsync()
        {
            return await _context.Clients.OrderByDescending(client => client.MaxOffer).ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByNewestUpdatedAsync()
        {
            return await _context.Clients.OrderByDescending(client => client.LastUpdated).ToListAsync();
        }

        public async Task<List<ClientModel>> GetClientsSortedByOldestUpdatedAsync()
        {
            return await _context.Clients.OrderBy(client => client.LastUpdated).ToListAsync();
        }


    }
}
