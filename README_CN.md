# RedisLock
[![NuGet](https://img.shields.io/nuget/v/Kirov.RedisLock.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Kirov.RedisLock)
[![Publish Workflow](https://github.com/kirov-opensource/Kirov.RedisLock/actions/workflows/publish.yml/badge.svg)](https://github.com/kirov-opensource/Kirov.RedisLock/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/dt/Kirov.RedisLock?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Kirov.RedisLock)
[![GitHub issues](https://img.shields.io/github/issues/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)](https://github.com/kirov-opensource/Kirov.RedisLock/issues)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)

StackExchange.Redis LockTake 的阻塞实现和锁的自动释放。

* [English](./README.md)

### How to use
* 从 `nuget` 引入 `Kirov.RedisLock` 包.

```csharp
public class Example
{
    private readonly IDatabase _redisDatabase;
    public Example(ConnectionMultiplexer connectionMultiplexer)
    {
        this._redisDatabase = connectionMultiplexer.GetDatabase();
    }

    public async Task ExampleFunction()
    {
        // 等待锁获取。
        await using (await _redisDatabase.BlockLockTakeAsync("key", "value"))
        {
            // 同步区间的代码。
        }
    }
}
```
完全不用关心锁的释放时机，在 using 方法块结束时会自动释放。它还有一些重载方法：
```csharp
// 这将导致锁在 3 分钟后被释放，即使 using 没有释放锁 (实际上，它只是将参数传递给 IDatabase.LockTakeAsync(expiry))。
BlockLockTakeAsync("key", "value", TimeSpan.FromMinutes(3));

// 锁在最多在三分钟后被释放。
// 如果没有获得锁，则每 200 毫秒再次尝试获得锁。
// 这个参数配置是 BlockLockTakeAsync() 方法的默认值。
BlockLockTakeAsync("key", "value", TimeSpan.FromMinutes(3), TimeSpan.FromMilliseconds(200));

// 获取锁时最多等待 5000 毫秒，超时则抛出 OperationCanceledException。
var cts = new CancellationTokenSource(5000);
BlockLockTakeAsync("key", "value", cts.Token);

```