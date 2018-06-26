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
        private readonly ITodoListTasksService _tasksService;

        public TodoListTaskController(ITodoListTasksService tasksService)
        {
            _tasksService = tasksService;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post(Guid id, [FromBody] TodoListTask task)
        {
            await _tasksService.AddTaskAsync(id, task, HttpContext.RequestAborted);

            return StatusCode(HttpStatusCode.Created);
        }

        [HttpPut("{taskId}/complete")]
        [ValidateModel]
        public async Task<ActionResult<CompletedTask>> Complete(Guid taskId, [FromBody]CompletedTask completedTask)
        {
            var task = await _tasksService.GetTaskAsync(taskId, HttpContext.RequestAborted);

            task.Completed = completedTask.Completed.Value;
            await _tasksService.UpdateTaskAsync(task, HttpContext.RequestAborted);

            return StatusCode(HttpStatusCode.Created);
        }
    }
}
