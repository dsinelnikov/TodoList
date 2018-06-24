using System;
using System.Collections.Generic;
using System.Text;

namespace InMemoryStorage
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
