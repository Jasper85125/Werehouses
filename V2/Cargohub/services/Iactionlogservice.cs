public interface Iactionlogservice{
    public List<ActionLogCS> GetLatestActionsForClients();
    public List<ActionLogCS> GetLatestActionsForInventoriers();
    public List<ActionLogCS> GetLatestActionsForItem_Groups();
    public List<ActionLogCS> GetLatestActionsForItem_Line();
    public List<ActionLogCS> GetLatestActionsForItem_Types();
    public List<ActionLogCS> GetLatestActionsForItems();
    public List<ActionLogCS> GetLatestActionsForLocations();
    public List<ActionLogCS> GetLatestActionsForOrders();
    public List<ActionLogCS> GetLatestActionsForShipments();
    public List<ActionLogCS> GetLatestActionsForSuppliers();
    public List<ActionLogCS> GetLatestActionsForTransfers();
    public List<ActionLogCS> GetLatestActionsForWarehouses();
    public void SaveActionLogs(List<ActionLogCS> actionLogs);
}