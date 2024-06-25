using FluentValidation;
using WebAPI.Grpc;

namespace WebAPI.Validators;

public class RemoveProductRequestValidator : AbstractValidator<RemoveProductRequest>
{
    public RemoveProductRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThanOrEqualTo(0);
    }
}