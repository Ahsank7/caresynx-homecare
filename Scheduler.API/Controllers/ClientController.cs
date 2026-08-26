using Scheduler.API.Common;
using Scheduler.API.Models.Client;
using Scheduler.API.Services.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : BaseController
    {
        private readonly IClient _Client;
        
        public ClientController(IClient Client, ILogger<ClientController> logger) 
            : base(logger)
        {
            _Client = Client;
        }

        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateClient(SaveClientInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Client data is required");

            return await ExecuteAsync(
                () => _Client.CreateUpdateClientAsync(model),
                "Client created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("GetClientDetails")]
        public async Task<IActionResult> GetClientDetails(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _Client.GetClientInfoAsync(UserId),
                "Client details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("GetClientList")]
        public async Task<IActionResult> GetClientList(ClientSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            return await ExecuteAsync(
                () => _Client.GetClientsAsync(model),
                "Client list retrieved successfully!"
            );
        }

        [HttpDelete]
        [Route("DeleteClient")]
        public async Task<IActionResult> DeleteClient(Guid Id)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid Client ID is required");

            return await ExecuteAsync(
                () => _Client.DeleteClientAsync(Id),
                "Client deleted successfully!"
            );
        }
    }
}
