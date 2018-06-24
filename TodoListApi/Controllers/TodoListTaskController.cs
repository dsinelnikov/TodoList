using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Filters;
using TodoListApi.Models;
using TodoListApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoListApi.Controllers
{
    [Route("lists/{id}/tasks")]
    public class TodoListTaskController : Controller
    {
        private readonly ITodoListService _listService;

        public TodoListTaskController(ITodoListService listService)
        {
            _listService = listService;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post(Guid id, [FromBody] TodoListTask task)
        {
            await _listService.AddTaskAsync(id, task, HttpContext.RequestAborted);

            return StatusCode(HttpStatusCode.Created);
        }

        [HttpPost("{taskId}/complete")]
        [ValidateModel]
        public async Task<ActionResult<CompletedTask>> Complete(Guid taskId, [FromBody]CompletedTask completedTask)
        {
            await _listService.CompleteTaskAsync(taskId, completedTask.Completed.Value, HttpContext.RequestAborted);

            return StatusCode(HttpStatusCode.Created);
        }
    }
}
