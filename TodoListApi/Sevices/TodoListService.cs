using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Core.Exceptions;
using TodoListApi.Models;
using TodoListApi.Storage;
using Dse = TodoListApi.Storage.Entities;

namespace TodoListApi.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly IStorageContext _storageContext;
        private readonly IMapper _mapper;

        public TodoListService(IStorageContext storageContext, IMapper mapper)
        {
            _storageContext = storageContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TodoList>> GetTodoListsAsync(GetTodoListQuery query, CancellationToken cancelationToken)
        {            
            IEnumerable<Dse.TodoList> items;

            if (!string.IsNullOrEmpty(query.Filter))
            {
                items = await _storageContext.TodoLists.Get(l => l.Name.Contains(query.Filter) || l.Description.Contains(query.Filter), cancelationToken);
            }
            else
            {
                items = await _storageContext.TodoLists.GetAll(cancelationToken);
            }
            
            items = items.OrderBy(l => l.Name)
                         .Skip(query.Offset)
                         .Take(query.Count);

            IEnumerable<TodoList> resultItems = _mapper.Map<List<TodoList>>(items);
            if (query.IncludeTasks)
            {
                cancelationToken.ThrowIfCancellationRequested();

                var listsDic = resultItems.ToDictionary(l => l.Id);
                var tasks = await _storageContext.Tasks.Get(t => listsDic.Keys.Contains(t.ListId));

                foreach (var list in listsDic.Values)
                {
                    list.Tasks = tasks.Where(t => t.ListId == list.Id)
                                    .Select(t => _mapper.Map<TodoListTask>(t))
                                    .OrderBy(t => t.Name)
                                    .ToList();
                }
            }

            cancelationToken.ThrowIfCancellationRequested();
            return resultItems;
        }

        public async Task<TodoList> GetTodoListAsync(Guid id, CancellationToken cancelationToken)
        {
            var dseItem = await _storageContext.TodoLists.Get(id, cancelationToken);

            if (dseItem == null)
            {
                throw new ItemNotFoundException(id);
            }

            var dseTasks = await _storageContext.Tasks.Get(t => t.ListId == id);

            var item = _mapper.Map<TodoList>(dseItem);
            item.Tasks = _mapper.Map<List<TodoListTask>>(dseTasks);            

            return item;
        }

        public async Task AddTodoListAsync(TodoList list, CancellationToken cancelationToken)
        {
            var dseList = _mapper.Map<Dse.TodoList>(list);
            
            await _storageContext.TodoLists.Add(dseList, cancelationToken);

            try
            {
                var dseTasks = _mapper.Map<List<Dse.TodoListTask>>(list.Tasks);

                await _storageContext.Tasks.AddRange(dseTasks, cancelationToken);
            }
            catch (ItemExistsException ex)
            {
                try
                {
                    await _storageContext.TodoLists.Remove(list.Id, cancelationToken);
                    throw;
                }
                catch (ItemExistsException)
                {
                    throw;
                }
                catch (Exception removeEx)
                {
                    throw new DataCorruptedException($"List with id '{list.Id}' was created, but tasl were not created. Because task with id '{ex.Id}' already exists.", removeEx);
                }
            }
        }

        public async Task AddTaskAsync(Guid listId, TodoListTask task, CancellationToken cancelationToken)
        {
            var dseList = await _storageContext.TodoLists.Get(listId, cancelationToken);
            if(dseList == null)
            {
                throw new ItemNotFoundException(listId);
            }

            var dseTask = _mapper.Map<Dse.TodoListTask>(task);
            dseTask.ListId = listId;
            await _storageContext.Tasks.Add(dseTask, cancelationToken);            
        }

        public async Task CompleteTaskAsync(Guid id, bool isCompleted, CancellationToken cancelationToken)
        {
            var dseTask = await _storageContext.Tasks.Get(id, cancelationToken);
            if(dseTask == null)
            {
                throw new ItemNotFoundException(id);
            }

            dseTask.Completed = isCompleted;

            await _storageContext.Tasks.Update(dseTask, cancelationToken);            
        }
    }
}