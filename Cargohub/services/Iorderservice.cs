namespace Services;

public interface IOrderService
{
    List<OrderCS> GetAllOrders();
    OrderCS GetOrderById(int id);

    OrderCS CreateOrder(OrderCS newOrder);
}