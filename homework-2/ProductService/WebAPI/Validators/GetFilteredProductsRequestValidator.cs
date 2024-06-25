using FluentValidation;
using WebAPI.Grpc;

namespace WebAPI.Validators;

public class GetFilteredProductsRequestValidator : AbstractValidator<GetFilteredProductsRequest>
{
    private const int MaxPageSize = 500;
    
    public GetFilteredProductsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).LessThan(MaxPageSize);
        RuleFor(x => x.WarehouseId)
            .Must(x => (x >= 0))
            .When(x => x.WarehouseId.HasValue);
    }
}