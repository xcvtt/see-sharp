using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WebAPI.Interceptors;

public class ValidationInterceptor : Interceptor
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        validator.ValidateAndThrow(request);
        
        return continuation(request, context);
    }
}