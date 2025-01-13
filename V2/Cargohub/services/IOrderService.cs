namespace ServicesV2;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    List<OrderCS> GetOrdersByClient(int client_id);
    List<OrderCS> GetOrdersByWarehouse(int warehouseId);
    OrderCS CreateOrder(OrderCS newOrder);
    List<OrderCS> CreateMultipleOrders(List<OrderCS> orders);
    Task<OrderCS> UpdateOrder(int id, OrderCS updateOrder);
    OrderCS PatchOrder(int id, string property, object newvalue);
    void DeleteOrder(int id);
    void DeleteOrders(List<int> ids);
    List<ItemIdAndAmount> GetItemsByOrderId(int orderId);
    Task<OrderCS> UpdateOrderItems(int orderId, List<ItemIdAndAmount> items);
}
