﻿using AutoMapper;
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
    public class TodoListTasksService : ITodoListTasksService
    {
        private readonly IStorageContext _storageContext;
        private readonly IMapper _mapper;

        public TodoListTasksService(IStorageContext storageContext, IMapper mapper)
        {
            _storageContext = storageContext;
            _mapper = mapper;
        }

        public async Task<TodoListTask> GetTaskAsync(Guid id, CancellationToken cancelationToken)
        {
            var task = await _storageContext.Tasks.Get(id, cancelationToken);
            if (task == null)
            {
                throw new ItemNotFoundException(id);
            }

            return _mapper.Map<TodoListTask>(task);
        }

        public async Task AddTaskAsync(Guid listId, TodoListTask task, CancellationToken cancelationToken)
        {
            var dseList = await _storageContext.TodoLists.Get(listId, cancelationToken);
            if (dseList == null)
            {
                throw new ItemNotFoundException(listId);
            }

            var dseTask = _mapper.Map<Dse.TodoListTask>(task);
            dseTask.ListId = listId;
            await _storageContext.Tasks.Add(dseTask, cancelationToken);
        }

        public async Task UpdateTaskAsync(TodoListTask task, CancellationToken cancelationToken)
        {
            await _storageContext.Tasks.Update(_mapper.Map<Dse.TodoListTask>(task));
        }        
    }
}
