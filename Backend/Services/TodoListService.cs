using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Core.Exceptions;
using TodoList.Backend.Models;
using TodoList.Backend.Storage;
using Dse = TodoList.Backend.Storage.Entities;

namespace TodoList.Backend.Services
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

        public async Task<IEnumerable<TodoListItem>> GetTodoListsAsync(GetTodoListQuery query, CancellationToken cancelationToken)
        {            
            IEnumerable<Dse.TodoListItem> items;

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

            IEnumerable<TodoListItem> resultItems = _mapper.Map<List<TodoListItem>>(items);
            if (query.IncludeTasks)
            {
                cancelationToken.ThrowIfCancellationRequested();

                var listsDic = resultItems.ToDictionary(l => l.Id);
                var tasks = await _storageContext.Tasks.Get(t => listsDic.Keys.Contains(t.ListId), cancelationToken);

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

        public async Task<TodoListItem> GetTodoListAsync(Guid id, CancellationToken cancelationToken)
        {
            var dseItem = await _storageContext.TodoLists.Get(id, cancelationToken);

            if (dseItem == null)
            {
                throw new ItemNotFoundException(id);
            }

            var dseTasks = await _storageContext.Tasks.Get(t => t.ListId == id);

            var item = _mapper.Map<TodoListItem>(dseItem);
            item.Tasks = _mapper.Map<List<TodoListTask>>(dseTasks);            

            return item;
        }

        public async Task AddTodoListAsync(TodoListItem list, CancellationToken cancelationToken)
        {
            var dseList = _mapper.Map<Dse.TodoListItem>(list);
            
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
    }
}