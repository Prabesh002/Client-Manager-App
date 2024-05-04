using Client_Manager_App_Models;

namespace Client_manager_Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<List<ClientModel>> GetAllClientsAsync();
        Task<ClientModel> GetClientByIdAsync(int id);
        Task<List<ClientModel>> SearchClientsAsync(string searchTerm);
        Task<List<ClientModel>> GetClientsByTypeAsync(ClientType clientType);
        Task<List<ClientModel>> GetFilteredClientsAsync(string searchTerm, string filterBy);
        Task<List<ClientModel>> GetClientsSortedByTimeEmailSentDescAsync();
        Task<List<ClientModel>> GetClientsSortedByTimeEmailSentAscAsync();
        Task<List<ClientModel>> GetClientsSortedByMaxOfferAscAsync();
        Task<List<ClientModel>> GetClientsSortedByMaxOfferDescAsync();
        Task<List<ClientModel>> GetClientsSortedByNewestUpdatedAsync();
        Task<List<ClientModel>> GetClientsSortedByOldestUpdatedAsync();
        Task<List<ClientModel>> GetClientsByCountryAsync(Country country);
        Task<List<ClientModel>> GetClientsByGenderAsync(Gender gender);
        Task<List<ClientModel>> GetClientsByEditingTypeAsync(string editingType);
        Task<List<ClientModel>> GetScammerClientsAsync();
        Task<List<ClientModel>> GetClientsWithAgencyAsync();
        Task<List<ClientModel>> GetClientsByPaymentTypeAsync(string paymentType);
        Task<List<ClientModel>> GetClientsByReWorkingRateAsync(string reWorkingRate);
    }


}
