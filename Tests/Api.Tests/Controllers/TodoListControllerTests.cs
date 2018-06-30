using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Backend.Models;
using TodoList.Backend.Services;
using TodoList.Controllers;
using Xunit;

namespace TodoList.Api.Tests.Controllers
{
    public class TodoListControllerTests
    {
        private readonly Mock<ITodoListService> _listService;
        private readonly HttpContext _httpContext;
        private readonly TodoListController _testController;

        public TodoListControllerTests()
        {
            _listService = new Mock<ITodoListService>(MockBehavior.Strict);
            _httpContext = new DefaultHttpContext();
            _testController = new TodoListController(_listService.Object);
            _testController.ControllerContext.HttpContext = _httpContext;
        }

        [Fact]
        public async Task GetAll_DefaultParameters()
        {
            // Arrange
            var expectedOffset = 0;
            var expectedCount = 25;
            var expectedFilter = string.Empty;
            var expectedIncludeTasks = false;
            var todoList = new TodoListItem
            {
                Id = Guid.NewGuid(),
                Name = "Item #1",
                Description = "Item #1 description"
            };
            _listService.Setup(s => s.GetTodoListsAsync(It.IsAny<GetTodoListQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new[] {todoList}.AsEnumerable()));

            // Act
            var response = await _testController.GetAll();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(response.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoListItem>>(actionResult.Value);
            Assert.Single(items);
            Assert.Equal(todoList, items.First());

            _listService.Verify(s => s.GetTodoListsAsync(It.Is<GetTodoListQuery>(q =>
                    q.Filter == expectedFilter
                 && q.Offset == expectedOffset
                 && q.Count == expectedCount    
                 && q.IncludeTasks == expectedIncludeTasks),
             _testController.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task GetAll_AllParameters()
        {
            // Arrange
            var expectedOffset = 5;
            var expectedCount = 17;
            var expectedFilter = "My filter";
            var expectedIncludeTasks = true;
            var todoList = new TodoListItem
            {
                Id = Guid.NewGuid(),
                Name = "Item #1",
                Description = "Item #1 description"
            };
            _listService.Setup(s => s.GetTodoListsAsync(It.IsAny<GetTodoListQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new[] { todoList }.AsEnumerable()));

            // Act
            var response = await _testController.GetAll(expectedFilter, expectedOffset, expectedCount, expectedIncludeTasks);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(response.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<TodoListItem>>(actionResult.Value);
            Assert.Single(items);
            Assert.Equal(todoList, items.First());

            _listService.Verify(s => s.GetTodoListsAsync(It.Is<GetTodoListQuery>(q =>
                    q.Filter == expectedFilter
                 && q.Offset == expectedOffset
                 && q.Count == expectedCount
                 && q.IncludeTasks == expectedIncludeTasks),
             _testController.HttpContext.RequestAborted));
        }

        [Fact]
        public async Task Get_ValidId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todoList = new TodoListItem
            {
                Id = id,
                Name = "Item #1",
                Description = "Item #1 description"
            };
            _listService.Setup(s => s.GetTodoListAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(todoList));

            // Act && Assert
            var response = await _testController.GetById(todoList.Id);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(response.Result);
            var item = Assert.IsAssignableFrom<TodoListItem>(actionResult.Value);            
            Assert.Equal(todoList, item);

            _listService.Verify(s => s.GetTodoListAsync(It.Is<Guid>(guid => guid == id), _testController.HttpContext.RequestAborted));
        }
    }
}
