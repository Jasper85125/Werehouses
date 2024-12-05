namespace ApiKeyAuthentication.Authentication;
public class ApiKeyModel
{
    public string Key {get; set;}
    public string Role {get; set;}
}

public class ApiKeyStorage
{
    public static List<ApiKeyModel> ApiKeys = new()
    {
        new ApiKeyModel {Key = "AdminKey", Role = "Admin"},
        new ApiKeyModel {Key = "WarehouseManagerKey", Role = "Warehouse Manager"},
        new ApiKeyModel {Key = "InventoryManagerKey", Role = "Inventory Manager"},
        new ApiKeyModel {Key = "FloorManagerKey", Role = "Floor Manager"},
        new ApiKeyModel {Key = "OperativeKey", Role = "Operative"},
        new ApiKeyModel {Key = "SupervisorKey", Role = "Supervisor"},
        new ApiKeyModel {Key = "AnalystKey", Role = "Analyst"},
        new ApiKeyModel {Key = "LogisticsKey", Role = "Logistics"},
        new ApiKeyModel {Key = "SalesKey", Role = "Sales"},
    };
}