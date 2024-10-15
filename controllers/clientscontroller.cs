using Microsoft.AspNetCore.Mvc;

namespace clients.controller{
    [Route("clients")]
    [ApiController]
    public class ClientsController: ControllerBase{
        private readonly Iclientservice _clientService;
        public ClientsController(Iclientservice iclientservice){
            _clientService = iclientservice;
        }
        
    }
}