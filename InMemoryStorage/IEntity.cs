namespace TodoList.InMemoryStorage
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
