﻿using Client_Manager_App_Models;

namespace Client_manager_Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<IQueryable<ClientModel>> GetAllClientsQueryableAsync();
        Task<ClientModel> GetClientByIdAsync(int id);
        Task<List<ClientModel>> SearchClientsAsync(string searchTerm);
        Task<List<ClientModel>> GetClientsByTypeAsync(ClientType clientType);
        Task<List<ClientModel>> GetFilteredClientsAsync(string searchTerm, string filterBy);
       
       
    }


}
