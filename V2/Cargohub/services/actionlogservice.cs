using Newtonsoft.Json;

namespace ServicesV2;
public class ActionLogService : Iactionlogservice{
    private string _logpath = "data/actionlogs.json";
    public List<ActionLogCS> GetAllActionLogs(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        var result = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();
        return result;
    }
    public List<ActionLogCS> GetLatestActionsForClients(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "client") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForInventories(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "inventory") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Groups(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?.GroupBy(_=>_.model == "item_group").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Line(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "item-line") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Types(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
    .GroupBy(_ => _.model)
        .Where(g => g.Key == "item-type") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForItems(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
    .GroupBy(_ => _.model)
        .Where(g => g.Key == "item") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForLocations(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "location") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForOrders(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "order") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForShipments(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "shipment") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForSuppliers(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "supplier") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForTransfers(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "transfer") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public List<ActionLogCS> GetLatestActionsForWarehouses(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json) ?? new List<ActionLogCS>();

        return actions?
        .GroupBy(_ => _.model)
        .Where(g => g.Key == "warehouse") // Ensure filtering specifically for clients
        .Select(g => g.OrderByDescending(a => a.timestamp).FirstOrDefault())
        .ToList() ?? new List<ActionLogCS>();
    }
    public void SaveActionLogs(List<ActionLogCS> actionLogs){
        var jsonactionlog = JsonConvert.SerializeObject(actionLogs, Formatting.Indented);
        File.WriteAllText(_logpath, jsonactionlog);
    }
}