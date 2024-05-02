using Client_Manager_App_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_manager_Repository.Interfaces
{
    internal interface IClientRepository 
    {
        Task<List<ClientModel>> GetAllClientsAsync();
        Task<ClientModel> GetClientByIdAsync(int id);
        Task<List<ClientModel>> SearchClientsAsync(string searchTerm);
    }
}
