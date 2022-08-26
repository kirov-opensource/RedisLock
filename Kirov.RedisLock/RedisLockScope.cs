using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Kirov.RedisLock
{
    public class RedisLockScope : IAsyncDisposable, IDisposable
    {
        private readonly IDatabase _database;
        private readonly RedisKey _redisKey;
        private readonly RedisValue _redisValue;
        private readonly TimeSpan _defaultDelay = TimeSpan.FromMilliseconds(200);
        private readonly TimeSpan _defaultkeyExpiry = TimeSpan.FromMinutes(3);

        private bool _disposed;

        public RedisLockScope(IDatabase database, RedisKey key, RedisValue value)
        {
            this._database = database;
            this._redisKey = key;
            this._redisValue = value;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LockDispose().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await LockDispose().ConfigureAwait(false);
        }

        private Task LockDispose()
        {
            if (_disposed)
            {
                return Task.CompletedTask;
            }
            _disposed = true;
            return _database.LockReleaseAsync(_redisKey, _redisValue);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public Task<RedisLockScope> LockAsync(CancellationToken cancellationToken = default)
        {
            return LockAsync(keyExpiry: _defaultkeyExpiry, delay: _defaultDelay, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public Task<RedisLockScope> LockAsync(TimeSpan keyExpiry, CancellationToken cancellationToken = default)
        {
            return LockAsync(keyExpiry: keyExpiry, delay: _defaultDelay, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="delay">Retry wait time for failed lock acquisition.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public Task<RedisLockScope> LockAsync(TimeSpan keyExpiry, TimeSpan delay, CancellationToken cancellationToken = default)
        {
            return LockAsync(keyExpiry: keyExpiry, delay: delay, flags: CommandFlags.None, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Blocking acquire lock.
        /// </summary>
        /// <param name="keyExpiry">The expiration of the lock key.</param>
        /// <param name="delay">Retry wait time for failed lock acquisition.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <param name="cancellationToken">If the token is cancelled, cancel the blocking wait.</param>
        /// <returns></returns>
        public async Task<RedisLockScope> LockAsync(TimeSpan keyExpiry, TimeSpan delay, CommandFlags flags, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            while (!await _database.LockTakeAsync(_redisKey, _redisValue, keyExpiry, flags).ConfigureAwait(false))
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }
            return this;
        }
    }
}

