using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WebAPI.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("Calling Type/Method: {Type} / {Method} with request {RequestType}: {Request}",
            MethodType.Unary, context.Method, request.GetType(), request);
        
        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation("Response is {ResponseType}: {Response}", response.GetType(), response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception thrown by {context.Method}");
            throw;
        }
    }
}