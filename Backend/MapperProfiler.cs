using AutoMapper;
using TodoList.Backend.Models;
using DB = TodoList.Backend.Storage.Entities;

namespace TodoList
{
    public class MapperProfiler : Profile
    {
        public MapperProfiler()
        {
            CreateMap<TodoListItem, DB.TodoListItem>();
            CreateMap<DB.TodoListItem, TodoListItem>();

            CreateMap<TodoListTask, DB.TodoListTask>();
            CreateMap<DB.TodoListTask, TodoListTask>();
        }
    }
}
