namespace ServicesV1;

public interface IClientService
{
    public List<ClientCS> GetAllClients();
    public ClientCS GetClientById(int clientId);
    public ClientCS CreateClient(ClientCS client);
    public ClientCS UpdateClient(int id, ClientCS client);
    void DeleteClient(int id);
}
