using Domain.ValueObjects;

namespace Domain.Entities;

public class Product
{
    public Product(
        ProductId productId,
        ProductName productName,
        ProductPrice productPrice,
        ProductWeight productWeight,
        ProductType productType,
        DateTime productCreatedDate,
        WarehouseId warehouseId)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentNullException.ThrowIfNull(productName);
        ArgumentNullException.ThrowIfNull(productPrice);
        ArgumentNullException.ThrowIfNull(productWeight);
        ArgumentNullException.ThrowIfNull(productType);
        ArgumentNullException.ThrowIfNull(productCreatedDate);
        ArgumentNullException.ThrowIfNull(warehouseId);
        
        ProductId = productId;
        ProductName = productName;
        ProductPrice = productPrice;
        ProductWeight = productWeight;
        ProductType = productType;
        ProductCreatedDate = productCreatedDate;
        WarehouseId = warehouseId;
    }

    public ProductId ProductId { get; }
    public ProductName ProductName { get; }
    public ProductPrice ProductPrice { get; }
    public ProductWeight ProductWeight { get; }
    public ProductType ProductType { get; }
    public DateTime ProductCreatedDate { get; }
    public WarehouseId WarehouseId { get; }

    public override string ToString()
    {
        return $"""
               ProductId: {ProductId}
               ProductName: {ProductName}
               ProductPrice: {ProductPrice}
               ProductWeight: {ProductWeight}
               ProductType: {ProductType}
               ProductCreatedDate: {ProductCreatedDate}
               WarehouseId: {WarehouseId}
               """;
    }
}