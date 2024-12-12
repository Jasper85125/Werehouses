using Newtonsoft.Json;

namespace ServicesV2;
public class ActionLogService : Iactionlogservice{
    private string _logpath = "data/actionlogs.json";
    public List<ActionLogCS> GetLatestActionsForClients(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "client").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForInventoriers(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "inventories").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Groups(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "item_group").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Line(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "item_line").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForItem_Types(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "item_type").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForItems(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "item").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForLocations(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "location").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForOrders(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "order").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForShipments(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "shipment").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForSuppliers(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "supplier").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForTransfers(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "transfer").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public List<ActionLogCS> GetLatestActionsForWarehouses(){
        if(!File.Exists(_logpath)){
            return new List<ActionLogCS>();
        }
        var json = File.ReadAllText(_logpath);
        List<ActionLogCS> actions = JsonConvert.DeserializeObject<List<ActionLogCS>>(json);

        return actions?.GroupBy(_=>_.model == "warehouse").Select(_ => _.OrderByDescending(_ => _.timestamp).FirstOrDefault()).ToList() ?? new List<ActionLogCS?>();
    }
    public void SaveActionLogs(List<ActionLogCS> actionLogs){
        var jsonactionlog = JsonConvert.SerializeObject(actionLogs, Formatting.Indented);
        File.WriteAllText(_logpath, jsonactionlog);
    }
}