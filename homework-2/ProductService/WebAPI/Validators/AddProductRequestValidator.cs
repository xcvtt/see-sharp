using FluentValidation;
using WebAPI.Grpc;

namespace WebAPI.Validators;

public class AddProductRequestValidator : AbstractValidator<AddProductRequest>
{
    public AddProductRequestValidator()
    {
        RuleFor(x => (decimal)x.ProductPrice).GreaterThanOrEqualTo(0M);
        RuleFor(x => (decimal)x.ProductWeight).GreaterThanOrEqualTo(0M);
        RuleFor(x => x.WarehouseId).GreaterThanOrEqualTo(0);
    }
}