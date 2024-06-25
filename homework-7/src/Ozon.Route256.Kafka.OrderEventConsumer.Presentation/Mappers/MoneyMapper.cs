using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;

public static class MoneyMapper
{
    private const int NanosMax = 1000_000_000;
    private const int CentsInUnit = 100;
    
    public static long AsLongCents(this OrderEvent.Money money)
    {
        return money.Units * CentsInUnit + money.Nanos / (NanosMax / CentsInUnit);
    }
}