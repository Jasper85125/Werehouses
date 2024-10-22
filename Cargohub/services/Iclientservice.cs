using Microsoft.AspNetCore.Mvc;

namespace Services;

public interface IClientService
{
    public List<ClientCS> GetAllClients();
    public ClientCS GetClientById(int clientId);
    public ClientCS CreateClient([FromBody] ClientCS newClientBody);
}
