using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services;

public class StatsService : IStatsService
{
    private readonly IItemStatsRepository _itemStatsRepository;
    private readonly ISellerStatsRepository _sellerStatsRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IItemStatsCalculator _itemStatsCalculator;
    private readonly ISellerStatsCalculator _sellerStatsCalculator;

    public StatsService(
        IUnitOfWork unitOfWork,
        IItemStatsRepository itemStatsRepository,
        ISellerStatsRepository sellerStatsRepository,
        IOrderRepository orderRepository,
        ISellerStatsCalculator sellerStatsCalculator,
        IItemStatsCalculator itemStatsCalculator)
    {
        _unitOfWork = unitOfWork;
        _itemStatsRepository = itemStatsRepository;
        _sellerStatsRepository = sellerStatsRepository;
        _orderRepository = orderRepository;
        _sellerStatsCalculator = sellerStatsCalculator;
        _itemStatsCalculator = itemStatsCalculator;
    }

    public async Task UpdateStats(IList<OrderEvent> orderEvents, CancellationToken token)
    {
        await _unitOfWork.Begin(token);

        try
        {
            var ordersToProcess = await GetOrdersToProcess(orderEvents, token);
            var itemStats = _itemStatsCalculator.CalculateItemStats(ordersToProcess);
            var sellerStats = _sellerStatsCalculator.CalculateSellerStats(ordersToProcess);
            await UpdateItemStats(itemStats, token);
            await UpdateSellerStats(sellerStats, token);

            await _unitOfWork.Commit(token);
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback(token);

            throw;
        }
    }
    
    private async Task UpdateItemStats(IEnumerable<ItemStats> itemStats, CancellationToken token)
    {
        var dbItemStats = itemStats.Select(x => x.AsDbEntity()).ToArray();
        
        await _itemStatsRepository.AddOrUpdate(dbItemStats, token);
    }

    private async Task UpdateSellerStats(IEnumerable<SellerStats> sellerStats, CancellationToken token)
    {
        var dbSellerStats = sellerStats.Select(x => x.AsDbEntity()).ToArray();

        await _sellerStatsRepository.AddOrUpdate(dbSellerStats, token);
    }

    private async Task<IList<OrderEvent>> GetOrdersToProcess(IList<OrderEvent> orderEvents, CancellationToken token)
    {
        var orders = orderEvents.Select(x => new Order
        {
            OrderId = x.OrderId,
            Status = (OrderStatus)x.Status,
        });
        
        var ordersToProcess = (await _orderRepository.AddReturn(orders.Select(x => x.AsDbEntity()).ToArray(), token))
            .Select(x => x.FromDbEntity())
            .ToHashSet();
        
        return orderEvents.Where(x => ordersToProcess.Contains(new Order
        {
            OrderId = x.OrderId,
            Status = (OrderStatus)x.Status,
        })).ToArray();
    }
}