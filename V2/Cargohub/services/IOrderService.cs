namespace ServicesV2;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    List<OrderCS> GetOrdersByClient(int client_id);
    List<OrderCS> GetOrdersByWarehouse(int warehouseId);
    List<OrderCS> GetOrdersByShipmentId(int shipmentId);
    OrderCS CreateOrder(OrderCS newOrder);
    List<OrderCS> CreateMultipleOrders(List<OrderCS> orders);
    OrderCS UpdateOrder(int id, OrderCS updateOrder);
    OrderCS PatchOrder(int id, string property, object newvalue);
    void DeleteOrder(int id);
    void DeleteOrders(List<int> ids);
    List<ItemIdAndAmount> GetItemsByOrderId(int orderId);
    OrderCS UpdateOrderItems(int orderId, List<ItemIdAndAmount> items);
}
