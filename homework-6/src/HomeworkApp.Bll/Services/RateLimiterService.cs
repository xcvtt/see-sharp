using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;

namespace HomeworkApp.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IDistributedLockFactory _lockFactory;
    private const int MaxRequests = 100;
    private const int CacheKeyExpirationInSeconds = 60;

    public RateLimiterService(IDistributedCache distributedCache, IDistributedLockFactory lockFactory)
    {
        _distributedCache = distributedCache;
        _lockFactory = lockFactory;
    }

    public async Task<int> GetRequestsLeftForUser(string userIpAddress, CancellationToken token)
    {
        const int lockExpireTimeSeconds = 20;
        const int lockWaitTimeSeconds = 10;
        const int lockRetryTimeSeconds = 1;

        await using var redlock = await _lockFactory.CreateLockAsync(
            userIpAddress,
            TimeSpan.FromSeconds(lockExpireTimeSeconds),
            TimeSpan.FromSeconds(lockWaitTimeSeconds),
            TimeSpan.FromSeconds(lockRetryTimeSeconds),
            token);

        if (redlock.IsAcquired)
        {
            var requestsLeft = await GetLimitForIp(userIpAddress, token);

            if (requestsLeft == -1)
            {
                return await SetLimitForIp(userIpAddress, token);
            }

            if (requestsLeft == 0)
            {
                return 0;
            }

            return await UpdateLimitForIp(userIpAddress, requestsLeft - 1, token);
        }

        return 0;
    }

    private async Task<int> GetLimitForIp(string userIpAddress, CancellationToken token)
    {
        var limitValue = await _distributedCache.GetStringAsync(userIpAddress, token);

        if (int.TryParse(limitValue, out var count))
        {
            return count;
        }

        return -1;
    }

    private async Task<int> SetLimitForIp(string userIpAddress, CancellationToken token)
    {
        await _distributedCache.SetStringAsync(
            userIpAddress,
            MaxRequests.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheKeyExpirationInSeconds)
            },
            token);

        return MaxRequests;
    }

    private async Task<int> UpdateLimitForIp(string userIpAddress, int newLimit, CancellationToken token)
    {
        await _distributedCache.SetStringAsync(
            userIpAddress,
            newLimit.ToString(),
            token);

        return newLimit;
    }
}