using FluentValidation;
using WebAPI.Grpc;

namespace WebAPI.Validators;

public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThanOrEqualTo(0);
    }
}