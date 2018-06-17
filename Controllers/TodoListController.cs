using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Services;
using TodoListApi.Models;
using TodoListApi.Filters;

namespace TodoListApi.Controllers
{
    [Route("lists")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly ITodoListService _listService;
        public TodoListController(ITodoListService listService)
        {
            _listService = listService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoList>>> GetAll(string searchString = "", int skip = 0, int limit = 25, bool includeTasks = false)
        {
            return await _listService.GetTodoListsAsync(new GetTodoListQuery
            {
              Offset = skip,
              Count = limit,
              Filter = searchString,
              IncludeTasks = includeTasks
            });
        }

        [HttpGet("{id}")]        
        public async Task<ActionResult<TodoList>> GetById(Guid id)
        {
          return await _listService.GetTodoListAsync(id);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]TodoList list)
        {
            await _listService.AddTodoListAsync(list);

            return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
        }
    }
}
