using Client_Manager_App_Models;

namespace Client_manager_Repository.Interfaces
{
    public interface IClientRepository 
    {
        Task<List<ClientModel>> GetAllClientsAsync();
        Task<ClientModel> GetClientByIdAsync(int id);
        Task<List<ClientModel>> SearchClientsAsync(string searchTerm);
    }
}
