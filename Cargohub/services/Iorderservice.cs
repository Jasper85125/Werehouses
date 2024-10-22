namespace Services;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);
    void CreateOrder(OrderCS newOrder);
}