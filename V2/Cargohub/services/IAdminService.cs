namespace ServicesV2;

public interface IAdminService
{
    string AddData(IFormFile file);
    string GenerateReport();
    ApiKeyModel UpdateAPIKeys(string ApiKey, ApiKeyModel NewApiKey);
    ApiKeyModel DeleteAPIKeys(string ApiKey);
}
