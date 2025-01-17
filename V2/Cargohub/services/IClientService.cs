namespace ServicesV2;

public interface IClientService
{
    public List<ClientCS> GetAllClients();
    public ClientCS GetClientById(int clientId);
    public ClientCS CreateClient(ClientCS client);
    List<ClientCS> CreateMultipleClients(List<ClientCS>newClients);
    public ClientCS UpdateClient(int id, ClientCS client);
    void DeleteClient(int id);
    void DeleteClients(List<int> ids);
    ClientCS PatchClient(int id, string property, object newValue);

}
