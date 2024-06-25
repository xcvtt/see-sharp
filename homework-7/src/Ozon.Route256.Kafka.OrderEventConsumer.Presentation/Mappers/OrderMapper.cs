using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;

public static class OrderMapper
{
    public static OrderEntityV1 AsDbEntity(this Order order)
    {
        return new OrderEntityV1
        {
            OrderId = order.OrderId,
            Status = (OrderStatusV1)order.Status,
        };
    }

    public static Order FromDbEntity(this OrderEntityV1 order)
    {
        return new Order
        {
            OrderId = order.OrderId,
            Status = (OrderStatus)order.Status,
        };
    }
}