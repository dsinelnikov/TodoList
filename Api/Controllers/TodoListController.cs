using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Backend.Models;
using TodoList.Backend.Services;
using TodoList.Filters;

namespace TodoList.Controllers
{
    [Route("v1/lists")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly ITodoListService _listService;
        public TodoListController(ITodoListService listService)
        {
            _listService = listService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoListItem>>> GetAll(string searchString = "", int skip = 0, int limit = 25, bool includeTasks = false)
        {
            var lists = await _listService.GetTodoListsAsync(new GetTodoListQuery
            {
                Offset = skip,
                Count = limit,
                Filter = searchString,
                IncludeTasks = includeTasks
            }, HttpContext.RequestAborted);

            return Ok(lists);                        
        }

        [HttpGet("{id}")]        
        public async Task<ActionResult<TodoListItem>> GetById(Guid id)
        {
          return await _listService.GetTodoListAsync(id, HttpContext.RequestAborted);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]TodoListItem list)
        {
            await _listService.AddTodoListAsync(list, HttpContext.RequestAborted);

            return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
        }
    }
}
