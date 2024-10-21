namespace Services;

public interface IClientService
{
    public List<ClientCS> GetAllClients();
    public ClientCS GetClientById(int clientId);
}
