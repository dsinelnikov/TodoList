using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InMemoryStorage
{
    public interface IEntityStorage<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        Task<TEntity> Get(TKey id, int millisecondsTimeout = 15000);

        Task<TEntity> Get(TKey id, CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task<IEnumerable<TEntity>> GetAll(int millisecondsTimeout = 15000);

        Task<IEnumerable<TEntity>> GetAll(CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, int millisecondsTimeout = 15000);

        Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> filter, CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task Add(TEntity entity, int millisecondsTimeout = 15000);

        Task Add(TEntity entity, CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task AddRange(IEnumerable<TEntity> entities, int millisecondsTimeout = 15000);

        Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task Update(TEntity entity, int millisecondsTimeout = 15000);

        Task Update(TEntity entity, CancellationToken cancelationToken, int millisecondsTimeout = 15000);

        Task<bool> Remove(TKey id, int millisecondsTimeout = 15000);

        Task<bool> Remove(TKey id, CancellationToken cancelationToken, int millisecondsTimeout = 15000);
    }
}
