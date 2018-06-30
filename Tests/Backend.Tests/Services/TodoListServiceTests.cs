using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Backend.Models;
using TodoList.Backend.Services;
using TodoList.Backend.Storage;
using TodoList.InMemoryStorage;
using Xunit;
using System.Linq;
using Dse = TodoList.Backend.Storage.Entities;

namespace TodoList.Backend.Tests.Services
{
    public class TodoListServiceTests
    {
        private readonly Mock<IEntityStorage<Dse.TodoListItem, Guid>> _todolists;
        private readonly Mock<IEntityStorage<Dse.TodoListTask, Guid>> _listTasks;
        private readonly Mock<IStorageContext> _storageContext;
        private readonly Mock<IMapper> _mapper;
        private readonly TodoListService _testService;

        public TodoListServiceTests()
        {
            _todolists = new Mock<IEntityStorage<Dse.TodoListItem, Guid>>(MockBehavior.Strict);
            _listTasks = new Mock<IEntityStorage<Dse.TodoListTask, Guid>>(MockBehavior.Strict);
            _storageContext = new Mock<IStorageContext>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _testService = new TodoListService(_storageContext.Object, _mapper.Object);

            _storageContext.SetupGet(s => s.TodoLists).Returns(_todolists.Object);
            _storageContext.SetupGet(s => s.Tasks).Returns(_listTasks.Object);
            _mapper.Setup(m => m.Map<List<TodoListItem>>(It.IsAny<IEnumerable<Dse.TodoListItem>>())).Returns<IEnumerable<Dse.TodoListItem>>(items =>
            {
                return items.Select(i => new TodoListItem
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description
                }).ToList();
            });
            _mapper.Setup(m => m.Map<TodoListTask>(It.IsAny<Dse.TodoListTask>())).Returns<Dse.TodoListTask>(item =>
            {
                return new TodoListTask
                {
                    Id = item.Id,
                    Name = item.Name,
                    Completed = item.Completed
                };
            });
        }
        
        [Fact]
        public async Task GetTodoListsAsync_GetAll()
        {
            // Arrange
            var storageLists = GenerateLists(25);
            var expectedItems = storageLists
                .OrderBy(item => item.Name)
                .ToList();
            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));

            // Act
            var actualItems = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = int.MaxValue
                },
                CancellationToken.None))
                .ToList();

            // Assert
            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.Equal(expectedItems.Select(i => i.Id), actualItems.Select(i => i.Id));
            Assert.Equal(expectedItems.Select(i => i.Name), actualItems.Select(i => i.Name));
            Assert.Equal(expectedItems.Select(i => i.Description), actualItems.Select(i => i.Description));
        }

        [Fact]
        public async Task GetTodoListsAsync_GetFirstItems()
        {
            // Arrange
            var expectedCount = 5;
            var storageLists = GenerateLists(25)
                .OrderBy(item => item.Name)
                .ToList();
            var expectedItems = storageLists
                .Take(expectedCount)
                .ToList();
            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));

            // Act
            var actualItems = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = expectedCount
                },
                CancellationToken.None))
                .ToList();

            // Assert
            Assert.Equal(expectedCount, actualItems.Count);
            Assert.Equal(expectedItems.Select(i => i.Id), actualItems.Select(i => i.Id));
            Assert.Equal(expectedItems.Select(i => i.Name), actualItems.Select(i => i.Name));
            Assert.Equal(expectedItems.Select(i => i.Description), actualItems.Select(i => i.Description));
        }

        [Fact]
        public async Task GetTodoListsAsync_GetRange()
        {
            // Arrange
            var expectedCount = 10;
            var expectedOffset = 5;
            var storageLists = GenerateLists(25)
                .OrderBy(item => item.Name)
                .ToList();
            var expectedItems = storageLists
                .Skip(5)
                .Take(expectedCount)
                .ToList();
            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));

            // Act
            var actualItems = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Offset = expectedOffset,
                    Count = expectedCount
                },
                CancellationToken.None))
                .ToList();

            // Assert
            Assert.Equal(expectedCount, actualItems.Count);
            Assert.Equal(expectedItems.Select(i => i.Id), actualItems.Select(i => i.Id));
            Assert.Equal(expectedItems.Select(i => i.Name), actualItems.Select(i => i.Name));
            Assert.Equal(expectedItems.Select(i => i.Description), actualItems.Select(i => i.Description));
        }

        [Fact]
        public async Task GetTodoListsAsync_GetByFilter()
        {
            // Arrange
            var filterStr = "Item 2";
            var storageLists = GenerateLists(25);
            var expectedItems = storageLists
                .Where(item => item.Name.Contains(filterStr))
                .OrderBy(item => item.Name)
                .ToList();
            _todolists.Setup(l => l.Get(It.IsAny<Func<Dse.TodoListItem, bool>>(), It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns<Func<Dse.TodoListItem, bool>, CancellationToken, TimeSpan?>(
                    (filter, token, timeout)
                        => Task.FromResult(storageLists.Where(item=>item.Name.Contains(filterStr))
                                                       .AsEnumerable())
                 );

            // Act
            var actualItems = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = int.MaxValue,
                    Filter = filterStr
                },
                CancellationToken.None))
                .ToList();

            // Assert
            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.Equal(expectedItems.Select(i => i.Id), actualItems.Select(i => i.Id));
            Assert.Equal(expectedItems.Select(i => i.Name), actualItems.Select(i => i.Name));
            Assert.Equal(expectedItems.Select(i => i.Description), actualItems.Select(i => i.Description));
        }

        [Fact]
        public async Task GetTodoListsAsync_GetAll_Canceled()
        {
            // Arrange
            var storageLists = GenerateLists(25);
            var expectedItems = storageLists
                .OrderBy(item => item.Name)
                .ToList();
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = int.MaxValue
                },
                tokenSource.Token));
            
        }

        [Fact]
        public async Task GetTodoListsAsync_GetAll_IncludeTasks()
        {
            // Arrange
            var storageLists = GenerateLists(1);
            var storageTask = new Dse.TodoListTask
            {
                Id = Guid.NewGuid(),
                Name = "Task #1",
                Completed = true,
                ListId = storageLists.First().Id
            };
            
            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));
            _listTasks.Setup(l => l.Get(It.IsAny<Func<Dse.TodoListTask, bool>>(), It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns<Func<Dse.TodoListTask, bool>, CancellationToken, TimeSpan?>(
                    (filter, token, timeout)
                        => Task.FromResult(new[] { storageTask}.AsEnumerable())
                 );

            // Act
            var actualItem = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = int.MaxValue,
                    IncludeTasks = true
                },
                CancellationToken.None))
                .First();

            // Assert
            Assert.Single(actualItem.Tasks);
            Assert.Equal(storageTask.Id, actualItem.Tasks[0].Id);
            Assert.Equal(storageTask.Name, actualItem.Tasks[0].Name);
            Assert.Equal(storageTask.Completed, actualItem.Tasks[0].Completed);
        }

        [Fact]
        public async Task GetTodoListsAsync_GetAll_NotIncludeTasts()
        {
            // Arrange
            var storageLists = GenerateLists(1);
            var storageTask = new Dse.TodoListTask
            {
                Id = Guid.NewGuid(),
                Name = "Task #1",
                Completed = true,
                ListId = storageLists.First().Id
            };

            _todolists.Setup(l => l.GetAll(It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.FromResult(storageLists.AsEnumerable()));
            _listTasks.Setup(l => l.Get(It.IsAny<Func<Dse.TodoListTask, bool>>(), It.IsAny<CancellationToken>(), It.IsAny<TimeSpan?>()))
                .Returns<Func<Dse.TodoListTask, bool>, CancellationToken, TimeSpan?>(
                    (filter, token, timeout)
                        => Task.FromResult(new[] { storageTask }.AsEnumerable())
                 );

            // Act
            var actualItem = (await _testService.GetTodoListsAsync(
                new GetTodoListQuery()
                {
                    Count = int.MaxValue,
                    IncludeTasks = false
                },
                CancellationToken.None))
                .First();

            // Assert
            Assert.Empty(actualItem.Tasks);
        }

        private static List<Dse.TodoListItem> GenerateLists(int count)
        {
            List<Dse.TodoListItem> items = new List<Dse.TodoListItem>();
            for (int i = 0; i < count; i++)
            {
                var item = new Dse.TodoListItem
                {
                    Id = Guid.NewGuid(),
                    Name = $"List {i}",
                    Description = $"List {i}"
                };                
                items.Add(item);
            }

            return items;
        }        
    }
}
