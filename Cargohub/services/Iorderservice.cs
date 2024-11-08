namespace Services;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    OrderCS CreateOrder(OrderCS newOrder);
    Task<OrderCS> UpdateOrder(int id, OrderCS updateOrder);
    void DeleteOrder(int id);
    List<ItemIdAndAmount> GetItemsByOrderId(int orderId);
}