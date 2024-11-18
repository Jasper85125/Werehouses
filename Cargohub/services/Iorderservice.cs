namespace Services;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    List<OrderCS> GetOrdersByClient(int client_id);
    OrderCS CreateOrder(OrderCS newOrder);
    Task<OrderCS> UpdateOrder(int id, OrderCS updateOrder);
    void DeleteOrder(int id);
    void DeleteOrders(List<int> ids);
    List<ItemIdAndAmount> GetItemsByOrderId(int orderId);
    Task<OrderCS> UpdateOrderItems(int orderId, List<ItemIdAndAmount> items);
}