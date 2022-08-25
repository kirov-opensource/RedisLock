using Moq;
using StackExchange.Redis;
using System.Diagnostics;

namespace Kirov.RedisLock.Tests;

public class RedisLockScopeTest
{
    [Fact]
    public async void When_LockTake_False_Must_Block_Waiting()
    {
        var databaseMock = new Mock<IDatabase>();

        databaseMock.SetupSequence(c => c.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), CommandFlags.None))
                    .ReturnsAsync(() =>
                    {
                        return false;
                    })
                    .ReturnsAsync(() =>
                    {
                        return false;
                    })
                    .ReturnsAsync(() =>
                    {
                        return true;
                    });

        var sw = new Stopwatch();
        sw.Start();
        await new RedisLockScope(databaseMock.Object, string.Empty, string.Empty).LockAsync(keyExpiry: TimeSpan.FromMinutes(5), delay: TimeSpan.FromSeconds(1));
        sw.Stop();
        Assert.True(sw.Elapsed.TotalMilliseconds >= 2 * 1000);
    }

    [Fact]
    public async void When_Cancellation_Then_Throw_Exception()
    {
        var cts = new CancellationTokenSource();
        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(c => c.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), CommandFlags.None))
                    .ReturnsAsync(() =>
                    {
                        cts.Cancel();
                        return false;
                    });

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await new RedisLockScope(databaseMock.Object, string.Empty, string.Empty).LockAsync(cts.Token);
        });
    }

    [Fact]
    public async void When_Dispose_Then_Release_Lock()
    {
        var release = false;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(c => c.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), CommandFlags.None))
                    .ReturnsAsync(() =>
                    {
                        return true;
                    });
        databaseMock.Setup(c => c.LockReleaseAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None))
            .ReturnsAsync(() =>
            {
                release = true;
                return release;
            });

        (await new RedisLockScope(databaseMock.Object, string.Empty, string.Empty).LockAsync()).Dispose();
        Assert.True(release);
    }

    [Fact]
    public async void When_DisposeAsync_Then_Release_Lock()
    {
        var release = false;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(c => c.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), CommandFlags.None))
                    .ReturnsAsync(() =>
                    {
                        return true;
                    });
        databaseMock.Setup(c => c.LockReleaseAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None))
            .ReturnsAsync(() =>
            {
                release = true;
                return release;
            });

        await (await new RedisLockScope(databaseMock.Object, string.Empty, string.Empty).LockAsync()).DisposeAsync();
        Assert.True(release);
    }
}
