using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using TodoListApi.Core.Exceptions;

namespace InMemoryStorage
{
    // I can use ConcurrentDictionary instead of this implementaion. But as I understand it's not interesting for this test task.
    public class EntityStorage<TEntity, TKey> : IEntityStorage<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        private readonly ReaderWriterLockSlim _accessLock = new ReaderWriterLockSlim();
        private readonly IDictionary<TKey, TEntity> _items = new Dictionary<TKey, TEntity>();

        public async Task Add(TEntity entity, int millisecondsTimeout)
        {
            await Add(entity, CancellationToken.None, millisecondsTimeout);
        }

        public async Task Add(TEntity entity, CancellationToken cancelationToken, int millisecondsTimeout)
        {            
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Task.Run(() => {
                if (_accessLock.TryEnterWriteLock(millisecondsTimeout))
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

        public async Task AddRange(IEnumerable<TEntity> entities, int millisecondsTimeout = 15000)
        {
            await AddRange(entities, CancellationToken.None, millisecondsTimeout);
        }

        public async Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancelationToken, int millisecondsTimeout = 15000)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            DateTime startTime = DateTime.Now;
            await Task.Run(() => {
                if (_accessLock.TryEnterUpgradeableReadLock(millisecondsTimeout))
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
                        millisecondsTimeout = GetMillisecondsLeftOrThrow(startTime, millisecondsTimeout);
                        if (_accessLock.TryEnterWriteLock(millisecondsTimeout))
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

        public async Task<TEntity> Get(TKey id, int millisecondsTimeout)
        {
            return await Get(id, CancellationToken.None, millisecondsTimeout);
        }

        public async Task<TEntity> Get(TKey id, CancellationToken cancelationToken, int millisecondsTimeout)
        {
            DateTime startDate = DateTime.Now;

            return await Task.Run(() => {
                if (_accessLock.TryEnterReadLock(millisecondsTimeout))
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        if (_items.TryGetValue(id, out var entity))
                        {
                            cancelationToken.ThrowIfCancellationRequested();
                            GetMillisecondsLeftOrThrow(startDate, millisecondsTimeout);
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

        public async Task<IEnumerable<TEntity>> GetAll(int millisecondsTimeout = 15000)
        {
            return await Get(null, CancellationToken.None, millisecondsTimeout);
        }

        public async Task<IEnumerable<TEntity>> GetAll(CancellationToken cancelationToken, int millisecondsTimeout = 15000)
        {
            return await Get(null, cancelationToken, millisecondsTimeout);
        }

        public async Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, int millisecondsTimeout)
        {
            return await Get(filter, CancellationToken.None, millisecondsTimeout);
        }

        public async Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, CancellationToken cancelationToken, int millisecondsTimeout)
        {
            DateTime startDate = DateTime.Now;

            return await Task.Run(() => {
                if (_accessLock.TryEnterReadLock(millisecondsTimeout))
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
                        GetMillisecondsLeftOrThrow(startDate, millisecondsTimeout);

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

        public async Task<bool> Remove(TKey id, int millisecondsTimeout)
        {
            return await Remove(id, CancellationToken.None, millisecondsTimeout);
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancelationToken, int millisecondsTimeout)
        {
            return await Task.Run(() => {
                if (_accessLock.TryEnterWriteLock(millisecondsTimeout))
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

        public async Task Update(TEntity entity, int millisecondsTimeout)
        {
            await Update(entity, CancellationToken.None, millisecondsTimeout);
        }

        public async Task Update(TEntity entity, CancellationToken cancelationToken, int millisecondsTimeout)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            DateTime startDate = DateTime.Now;
            await Task.Run(() => {
                if (_accessLock.TryEnterUpgradeableReadLock(millisecondsTimeout))
                {
                    millisecondsTimeout = GetMillisecondsLeftOrThrow(startDate, millisecondsTimeout);

                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        if (_items.ContainsKey(entity.Id))
                        {
                            if (_accessLock.TryEnterWriteLock(millisecondsTimeout))
                            {
                                try
                                {
                                    cancelationToken.ThrowIfCancellationRequested();
                                    GetMillisecondsLeftOrThrow(startDate, millisecondsTimeout);

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

        private static int GetMillisecondsLeftOrThrow(DateTime startDate, int millisecondsTimeout)
        {
            var timeLeft = TimeSpan.FromMilliseconds(millisecondsTimeout) - (DateTime.Now - startDate);

            if(timeLeft.TotalMilliseconds < 0)
            {
                throw new TimeoutException();
            }

            return (int)timeLeft.TotalMilliseconds;
        }
    }
}
