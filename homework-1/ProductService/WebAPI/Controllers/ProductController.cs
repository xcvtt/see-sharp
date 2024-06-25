using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos;
using Application.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/products/")]
public class ProductController : ControllerBase
{
    private readonly ISalesPredictionService _salesPredictionService;

    public ProductController(ISalesPredictionService salesPredictionService)
    {
        _salesPredictionService = salesPredictionService;
    }

    private CancellationToken CancellationToken => HttpContext.RequestAborted;
    

    [HttpGet("{productId:int}/ads")]
    public async Task<ActionResult<CalculateAds>> CalculateAverageDailySales(int productId)
    {
        var ads = await _salesPredictionService.CalculateAverageDailySalesAsync(productId, CancellationToken);
        return Ok(ads);
    }

    [HttpGet("{productId:int}/prediction")]
    public async Task<ActionResult<CalculatePrediction>> CalculateSalesPrediction(int productId, int days)
    {
        var sales = await _salesPredictionService.CalculateSalesPredictionAsync(productId, days, CancellationToken);
        return Ok(sales);
    }

    [HttpGet("{productId:int}/demand")]
    public async Task<ActionResult<CalculateDemand>> CalculateProductDemand(int productId, int days)
    {
        var demand = await _salesPredictionService.CalculateProductDemandAsync(productId, days, CancellationToken);
        return Ok(demand);
    }
}