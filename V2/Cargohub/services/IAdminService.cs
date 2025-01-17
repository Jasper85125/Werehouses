namespace ServicesV2;

public interface IAdminService
{
    string AddData(IFormFile file);
    string GenerateReport();
    ApiKeyModel UpdateAPIKeys(string ApiKey, ApiKeyModel NewApiKey);
    ApiKeyModel AddAPIKeys(ApiKeyModel ApiKey);
}
