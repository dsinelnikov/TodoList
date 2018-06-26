using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InMemoryStorage
{
    public interface IEntityStorage<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        Task<TEntity> Get(TKey id, TimeSpan? timeout = null);

        Task<TEntity> Get(TKey id, CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task<IEnumerable<TEntity>> GetAll(TimeSpan? timeout = null);

        Task<IEnumerable<TEntity>> GetAll(CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, TimeSpan? timeout = null);

        Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task Add(TEntity entity, TimeSpan? timeout = null);

        Task Add(TEntity entity, CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task AddRange(IEnumerable<TEntity> entities, TimeSpan? timeout = null);

        Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task Update(TEntity entity, TimeSpan? timeout = null);

        Task Update(TEntity entity, CancellationToken cancelationToken, TimeSpan? timeout = null);

        Task<bool> Remove(TKey id, TimeSpan? timeout = null);

        Task<bool> Remove(TKey id, CancellationToken cancelationToken, TimeSpan? timeout = null);
    }
}
