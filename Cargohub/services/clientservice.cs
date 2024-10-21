using Newtonsoft.Json;

namespace clients.Services{
    public class ClientService: Iclientservice{
        public ClientService(){}
        public List<ClientCS> GetAllClients()
        {
            var dataPath = "/data/clients.json";
            if(!File.Exists(dataPath))
            {
                return new List<ClientCS>();
            }
            var json = File.ReadAllText(dataPath);
            var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
            return clientsdata ?? new List<ClientCS>();
        }
    }
}
