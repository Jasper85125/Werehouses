namespace ServicesV1;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    List<OrderCS> GetOrdersByClient(int client_id);
    List<OrderCS> GetOrdersByShipmentId(int shipmentId);
    OrderCS CreateOrder(OrderCS newOrder);
    OrderCS UpdateOrder(int id, OrderCS updateOrder);
    void DeleteOrder(int id);
    List<ItemIdAndAmount> GetItemsByOrderId(int orderId);
    OrderCS UpdateOrderItems(int orderId, List<ItemIdAndAmount> items);
}
