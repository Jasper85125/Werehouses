using Newtonsoft.Json;

namespace clients.Services{
    public class ClientService: Iclientservice{
        public ClientService(){}
        private string dataPath = "/data/clients.json";
        public List<ClientCS> GetAllClients()
        {
            if(!File.Exists(dataPath))
            {
                return new List<ClientCS>();
            }
            var json = File.ReadAllText(dataPath);
            var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
            return clientsdata ?? new List<ClientCS>();
        }
        public ClientCS? GetClientById(int id){
            var json = File.ReadAllText(dataPath);
            var clientdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
            if(clientdata[id].Id == id)
            {
                return clientdata[id];
            }
            return null;
        }
    }
}
