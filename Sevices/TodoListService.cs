using System;
using System.Collections.Generic;
using System.Linq;
using TodoListApi.Models;
using TodoListApi.Exceptions;
using DB = TodoListApi.Storage;
using Dbe = TodoListApi.Storage.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Data.SqlClient;
using TodoListApi.ExceptionHandling;

namespace TodoListApi.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly DB.ApiDbContext _apiDbContext;
        private readonly IMapper _mapper;

        public TodoListService(DB.ApiDbContext apiDbContext, IMapper mapper)
        {
            _apiDbContext = apiDbContext;
            _mapper = mapper;
        }

        public async Task<List<TodoList>> GetTodoListsAsync(GetTodoListQuery query)
        {
            var items = _apiDbContext.TodoLists.AsQueryable();

            if (!string.IsNullOrEmpty(query.Filter))
            {
                items = items.Where(l => l.Name.Contains(query.Filter) || l.Description.Contains(query.Filter));
            }

            items = items.OrderBy(l => l.Name)
                         .Skip(query.Offset)
                         .Take(query.Count);

            IQueryable<TodoList> resultItems;
            if (query.IncludeTasks)
            {
                resultItems = items.Select(l => new TodoList
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    Tasks = l.Tasks
                        .OrderBy(t => t.Name)
                        .Select(t => new TodoListTask
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Completed = t.Completed
                        }).ToList()
                });
            }
            else
            {
                resultItems = items.Select(l => new TodoList
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description
                });
            }

            return await resultItems.ToListAsync();
        }

        public async Task<TodoList> GetTodoListAsync(Guid id)
        {
            var item = await _apiDbContext.TodoLists
                          .Where(l => l.Id == id)
                          .Select(l => new TodoList
                          {
                              Id = l.Id,
                              Name = l.Name,
                              Description = l.Description,
                              Tasks = l.Tasks.Select(t => new TodoListTask
                              {
                                  Id = t.Id,
                                  Name = t.Name,
                                  Completed = t.Completed
                              }).ToList()
                          })
                          .FirstOrDefaultAsync();

            if (item == null)
            {
                throw new ItemNotFoundException(id);
            }

            return item;
        }

        public async Task AddTodoListAsync(TodoList list)
        {
            var dbList = _mapper.Map<Dbe.TodoList>(list);

            _apiDbContext.TodoLists.Add(dbList);
            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (SqlExceptionHelper.TyGetExceptionType(ex, out var type))
            {
                switch (type)
                {
                    case SqlExceptionType.DublicateKey:
                        throw new ItemExistsException("Some task is already exists.", ex);
                    default:
                        throw;
                }
            }
        }

        public async Task AddTaskAsync(Guid listId, TodoListTask task)
        {
            var dbTask = _mapper.Map<Dbe.TodoListTask>(task);
            dbTask.TodoList = new Dbe.TodoList { Id = listId };

            _apiDbContext.TodoLists.Attach(dbTask.TodoList);
            _apiDbContext.Tasks.Add(dbTask);

            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (SqlExceptionHelper.TyGetExceptionType(ex, out var type))
            {
                switch (type)
                {
                    case SqlExceptionType.Constraint:
                        throw new ItemNotFoundException(listId, ex);
                    case SqlExceptionType.DublicateKey:
                        throw new ItemExistsException(task.Id, ex);
                    default:
                        throw;
                }                
            }
        }

        public async Task CompleteTaskAsync(Guid id, bool isCompleted)
        {
            var dbTask = new Dbe.TodoListTask { Id = id, Completed = isCompleted };
            
            _apiDbContext.Entry(dbTask).Property(e => e.Completed).IsModified = true;

            try
            {
                await _apiDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (SqlExceptionHelper.TyGetExceptionType(ex, out var type))
            {
                switch (type)
                {
                    case SqlExceptionType.Concurrency:
                        throw new ItemNotFoundException(id, ex);
                    default:
                        throw;
                }
            }
        }
    }
}