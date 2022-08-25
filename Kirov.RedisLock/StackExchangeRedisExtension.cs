using System;
using Kirov.RedisLock;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis
{
    public static class StackExchangeRedisExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database">Redis database.</param>
        /// <param name="redisKey">The key of the lock.</param>
        /// <param name="redisValue">The value to set at the key.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public static Task<RedisLockScope> BlockLockTakeAsync(this IDatabase database, RedisKey redisKey, RedisValue redisValue, CancellationToken cancellationToken = default)
        {
            return new RedisLockScope(database, redisKey, redisValue).LockAsync(cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="database">Redis database.</param>
        /// <param name="redisKey">The key of the lock.</param>
        /// <param name="redisValue">The value to set at the key.</param>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public static Task<RedisLockScope> BlockLockTakeAsync(this IDatabase database, RedisKey redisKey, RedisValue redisValue, TimeSpan keyExpiry, CancellationToken cancellationToken = default)
        {
            return new RedisLockScope(database, redisKey, redisValue).LockAsync(keyExpiry, cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="database">Redis database.</param>
        /// <param name="redisKey">The key of the lock.</param>
        /// <param name="redisValue">The value to set at the key.</param>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="delay">Retry wait time for failed lock acquisition.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public static Task<RedisLockScope> BlockLockTakeAsync(this IDatabase database, RedisKey redisKey, RedisValue redisValue, TimeSpan keyExpiry, TimeSpan delay, CancellationToken cancellationToken = default)
        {
            return new RedisLockScope(database, redisKey, redisValue).LockAsync(keyExpiry, delay, cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="database">Redis database.</param>
        /// <param name="redisKey">The key of the lock.</param>
        /// <param name="redisValue">The value to set at the key.</param>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="delay">Retry wait time for failed lock acquisition.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public static Task<RedisLockScope> BlockLockTakeAsync(this IDatabase database, RedisKey redisKey, RedisValue redisValue, TimeSpan keyExpiry, TimeSpan delay, CommandFlags flags, CancellationToken cancellationToken = default)
        {
            return new RedisLockScope(database, redisKey, redisValue).LockAsync(keyExpiry, delay, flags, cancellationToken);
        }
    }
}
