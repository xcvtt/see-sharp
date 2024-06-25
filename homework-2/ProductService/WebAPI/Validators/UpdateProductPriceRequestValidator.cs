using FluentValidation;
using WebAPI.Grpc;

namespace WebAPI.Validators;

public class UpdateProductPriceRequestValidator : AbstractValidator<UpdateProductPriceRequest>
{
    public UpdateProductPriceRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThanOrEqualTo(0);
        RuleFor(x => (decimal)x.ProductPrice).GreaterThanOrEqualTo(0M);
    }
}