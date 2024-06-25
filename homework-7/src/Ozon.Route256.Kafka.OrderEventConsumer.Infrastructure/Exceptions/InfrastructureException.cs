using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Exceptions;

public class InfrastructureException(string msg) : Exception(msg);