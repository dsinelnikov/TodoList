using TodoList.InMemoryStorage;

namespace InMemoryStorage.Tests
{
    class MockEntity : IEntity<int>
    {
        public int Id { get; set; }

        public MockEntity(int id)
        {
            Id = id;
        }
    }
}
