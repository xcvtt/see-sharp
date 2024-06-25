using System.Text;
using AutoFixture.Xunit2;
using FluentAssertions;
using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using RedLockNet;
using RedLockNet.SERedis;
using Xunit;

namespace HomeworkApp.UnitTests;

public class RateLimiterServiceTest
{
    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly Mock<IDistributedLockFactory> _lockMock = new();
    private readonly RateLimiterService _rateLimiterService;

    public RateLimiterServiceTest()
    {
        _rateLimiterService = new RateLimiterService(_cacheMock.Object, _lockMock.Object);

        var redlock = new Mock<IRedLock>();
        redlock
            .Setup(x => x.IsAcquired)
            .Returns(true);

        _lockMock
            .Setup(x => x.CreateLockAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(redlock.Object);
    }

    [Theory, AutoData]
    public async Task GetRequestsLeftForUser_MoreRequestsLeft_ReturnsRequestsLeftMinusOne(string userIpAddress)
    {
        // Arrange
        const int requestsLeft = 123;
        _cacheMock
            .Setup(x => x.GetAsync(userIpAddress, default))
            .ReturnsAsync(() => Encoding.UTF8.GetBytes(requestsLeft.ToString()));

        // Act
        var requests = await _rateLimiterService.GetRequestsLeftForUser(userIpAddress, default);

        // Assert
        requests.Should().Be(requestsLeft-1);
        _cacheMock.Verify(x => x.GetAsync(userIpAddress, default), Times.Once);
        _cacheMock.Verify(x => x.SetAsync(
            userIpAddress,
            Encoding.UTF8.GetBytes((requestsLeft-1).ToString()),
            It.IsAny<DistributedCacheEntryOptions>(),
            default), Times.Once);
    }
    
    [Theory, AutoData]
    public async Task GetRequestsLeftForUser_NoRequestsLeft_ReturnsZero(string userIpAddress)
    {
        // Arrange
        const int requestsLeft = 0;
        _cacheMock
            .Setup(x => x.GetAsync(userIpAddress, default))
            .ReturnsAsync(() => Encoding.UTF8.GetBytes(requestsLeft.ToString()));

        // Act
        var requests = await _rateLimiterService.GetRequestsLeftForUser(userIpAddress, default);

        // Assert
        requests.Should().Be(requestsLeft);
        _cacheMock.Verify(x => x.GetAsync(userIpAddress, default), Times.Once);
        _cacheMock.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            default), Times.Never);
    }
}