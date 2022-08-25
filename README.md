# RedisLock
[![NuGet](https://img.shields.io/nuget/v/Kirov.RedisLock.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Kirov.RedisLock)
[![Publish Workflow](https://github.com/kirov-opensource/Kirov.RedisLock/actions/workflows/publish.yml/badge.svg)](https://github.com/kirov-opensource/Kirov.RedisLock/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/dt/Kirov.RedisLock?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Kirov.RedisLock)
[![GitHub issues](https://img.shields.io/github/issues/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)](https://github.com/kirov-opensource/Kirov.RedisLock/issues)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/kirov-opensource/Kirov.RedisLock.svg?style=flat-square&logo=github)

The blocking implementation of StackExchange.Redis LockTake and the automatic release of locks.

* [中文](./README_CN.md)

### How to use
* Introducing `Kirov.RedisLock` in the nuget.

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
        // Waiting for lock acquisition.
        await using (await _redisDatabase.BlockLockTakeAsync("key", "value"))
        {
            // Your code here.
        }
    }
}
```
Don't care about the release timing of the lock at all, it will be automatically released at the end of the using method block. It also has several overloaded methods: 
```csharp
// This will cause the lock to be released after 3 minute, even if the using does not release the lock (actually, it passes the parameter to IDatabase.LockTakeAsync(expiry)).
BlockLockTakeAsync("key", "value", TimeSpan.FromMinutes(3));

// This will cause the lock to be released after 3 minute.
// If the lock is not acquired, try to acquire it again every 200ms.
// This parameter is actually the default value of BlockLockTakeAsync().
BlockLockTakeAsync("key", "value", TimeSpan.FromMinutes(3), TimeSpan.FromMilliseconds(200));

// Wait up to 5000 milliseconds when trying to acquire a lock, throw OperationCanceledException if the timeout expires.
var cts = new CancellationTokenSource(5000);
BlockLockTakeAsync("key", "value", cts.Token);

```