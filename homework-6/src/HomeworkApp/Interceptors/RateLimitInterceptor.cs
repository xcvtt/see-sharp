using Grpc.Core;
using Grpc.Core.Interceptors;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Exceptions;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Interceptors;

public class RateLimitInterceptor : Interceptor
{
    private readonly IRateLimiterService _rateLimiterService;

    public RateLimitInterceptor(IRateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var userIpAddress = context.RequestHeaders.Get("X-R256-USER-IP")?.Value;

        if (userIpAddress is null)
        {
            return await continuation(request, context);
        }

        var requestsLeft = await _rateLimiterService.GetRequestsLeftForUser(userIpAddress, context.CancellationToken);
        
        if (requestsLeft <= 0)
        {
            throw new RateLimitExceededException(
                $"User with ip {userIpAddress} exceeded the limit of requests");
        }
        
        return await continuation(request, context);
    }
}