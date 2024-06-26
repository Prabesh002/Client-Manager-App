﻿using System.Collections.Generic;
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

		public async Task<IQueryable<ClientModel>> GetAllClientsQueryableAsync()
		{
			return await Task.FromResult(_context.Clients.AsQueryable());
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
					   query = query.Where(c => c.Name.Contains(searchTerm));
						break;
				}
			}

			return await query.ToListAsync();
		}

	   
	}
}
