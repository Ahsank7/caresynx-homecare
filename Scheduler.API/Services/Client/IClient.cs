using Scheduler.API.Models.Client;

namespace Scheduler.API.Services.Client
{
    public interface IClient
    {
        Task<ClientSearchResponse> GetClientsAsync(ClientSearchRequest request);   
        Task<ClientInfo> GetClientInfoAsync(Guid userId);  
        Task<Guid?> CreateUpdateClientAsync(SaveClientInfoViewModel savePatientInfoViewModel);
        Task<Guid> DeleteClientAsync(Guid id);
        
        // Keep old sync method for backward compatibility
        Guid DeleteClient(Guid id);    
    }
}
