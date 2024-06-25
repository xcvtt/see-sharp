namespace HomeworkApp.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    Task<int> GetRequestsLeftForUser(string userIpAddress, CancellationToken token);
}