using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using TodoListApi.Core.Exceptions;
using System.Diagnostics;

namespace InMemoryStorage
{
    // I can use ConcurrentDictionary instead of this implementaion. But as I understand it's not interesting for this test task.
    public class EntityStorage<TEntity, TKey> : IEntityStorage<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(15);
        private readonly ReaderWriterLockSlim _accessLock = new ReaderWriterLockSlim();
        private readonly IDictionary<TKey, TEntity> _items = new Dictionary<TKey, TEntity>();

        public async Task Add(TEntity entity, TimeSpan? timeout)
        {
            await Add(entity, CancellationToken.None, timeout);
        }

        public async Task Add(TEntity entity, CancellationToken cancelationToken, TimeSpan? timeout = null)
        {            
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Task.Run(() => {
                if (_accessLock.TryEnterWriteLock(timeout ?? _defaultTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        _items.Add(entity.Id, entity);
                    }
                    catch(ArgumentException ex)
                    {
                        throw new ItemExistsException(entity.Id, ex);
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            }, cancelationToken);
        }

        public async Task AddRange(IEnumerable<TEntity> entities, TimeSpan? timeout)
        {
            await AddRange(entities, CancellationToken.None, timeout);
        }

        public async Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancelationToken, TimeSpan? timeout)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var startTime = Stopwatch.StartNew();
            var operationTimeout = timeout ?? _defaultTimeout;
            await Task.Run(() => {
                if (_accessLock.TryEnterUpgradeableReadLock(operationTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        var existsItem = entities.FirstOrDefault(e => _items.ContainsKey(e.Id));
                        if(existsItem != null)
                        {
                            throw new ItemExistsException(existsItem.Id);
                        }

                        cancelationToken.ThrowIfCancellationRequested();
                        var currentTimeout = GetTimeoutLeftOrThrow(startTime, operationTimeout);
                        if (_accessLock.TryEnterWriteLock(currentTimeout))
                        {
                            try
                            {
                                foreach (var entity in entities)
                                {
                                    _items.Add(entity.Id, entity);
                                }
                            }
                            finally
                            {
                                _accessLock.ExitWriteLock();
                            }
                        }
                        else
                        {
                            throw new TimeoutException();
                        }
                    }
                    finally
                    {
                        _accessLock.ExitUpgradeableReadLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            }, cancelationToken);
        }

        public async Task<TEntity> Get(TKey id, TimeSpan? timeout = null)
        {
            return await Get(id, CancellationToken.None, timeout);
        }

        public async Task<TEntity> Get(TKey id, CancellationToken cancelationToken, TimeSpan? timeout = null)
        {
            var startTime = Stopwatch.StartNew();
            var operationTimeout = timeout ?? _defaultTimeout;

            return await Task.Run(() => {
                if (_accessLock.TryEnterReadLock(operationTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        if (_items.TryGetValue(id, out var entity))
                        {
                            cancelationToken.ThrowIfCancellationRequested();
                            GetTimeoutLeftOrThrow(startTime, operationTimeout);
                            return entity;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    finally
                    {
                        _accessLock.ExitReadLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            }, cancelationToken);            
        }

        public async Task<IEnumerable<TEntity>> GetAll(TimeSpan? timeout = null)
        {
            return await Get(null, CancellationToken.None, timeout);
        }

        public async Task<IEnumerable<TEntity>> GetAll(CancellationToken cancelationToken, TimeSpan? timeout = null)
        {
            return await Get(null, cancelationToken, timeout);
        }

        public async Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, TimeSpan? timeout = null)
        {
            return await Get(filter, CancellationToken.None, timeout);
        }

        public async Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, CancellationToken cancelationToken, TimeSpan? timeout = null)
        {
            var startTime = Stopwatch.StartNew();
            var operationTimeout = timeout ?? _defaultTimeout;

            return await Task.Run(() => {
                if (_accessLock.TryEnterReadLock(operationTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();

                        List<TEntity> result;
                        if (filter == null)
                        {
                            result = _items.Values.ToList();
                        }
                        else
                        {
                            result = _items.Values.Where(filter).ToList();
                        }

                        cancelationToken.ThrowIfCancellationRequested();
                        GetTimeoutLeftOrThrow(startTime, operationTimeout);

                        return result;
                    }
                    finally
                    {
                        _accessLock.ExitReadLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            });            
        }

        public async Task<bool> Remove(TKey id, TimeSpan? timeout = null)
        {
            return await Remove(id, CancellationToken.None, timeout);
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancelationToken, TimeSpan? timeout = null)
        {
            return await Task.Run(() => {
                if (_accessLock.TryEnterWriteLock(timeout ?? _defaultTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        return _items.Remove(id);
                    }
                    finally
                    {
                        _accessLock.ExitWriteLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            });            
        }

        public async Task Update(TEntity entity, TimeSpan? timeout = null)
        {
            await Update(entity, CancellationToken.None, timeout);
        }

        public async Task Update(TEntity entity, CancellationToken cancelationToken, TimeSpan? timeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var startTime = Stopwatch.StartNew();
            var operationTimeout = timeout ?? _defaultTimeout;

            await Task.Run(() => {
                if (_accessLock.TryEnterUpgradeableReadLock(operationTimeout))
                {
                    try
                    {
                        var currentTimeout = GetTimeoutLeftOrThrow(startTime, operationTimeout);
                        cancelationToken.ThrowIfCancellationRequested();

                        if (_items.ContainsKey(entity.Id))
                        {
                            currentTimeout = GetTimeoutLeftOrThrow(startTime, operationTimeout);
                            if (_accessLock.TryEnterWriteLock(currentTimeout))
                            {
                                try
                                {
                                    cancelationToken.ThrowIfCancellationRequested();                                    

                                    _items[entity.Id] = entity;
                                }
                                finally
                                {
                                    _accessLock.ExitWriteLock();
                                }
                            }
                        }
                        else
                        {
                            throw new ItemNotFoundException(entity.Id);
                        }
                    }
                    finally
                    {
                        _accessLock.ExitUpgradeableReadLock();
                    }
                }
                else
                {
                    throw new TimeoutException();
                }
            });            
        }

        private static TimeSpan GetTimeoutLeftOrThrow(Stopwatch elapsedTime, TimeSpan timeout)
        {
            var timeLeft = timeout - TimeSpan.FromTicks(elapsedTime.ElapsedTicks);

            if(timeLeft.Ticks < 0)
            {
                throw new TimeoutException();
            }

            return timeLeft;
        }
    }
}
