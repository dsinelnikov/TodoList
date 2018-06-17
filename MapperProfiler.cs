using AutoMapper;
using TodoListApi.Models;
using DB = TodoListApi.Storage.Entities;

namespace TodoListApi
{
    public class MapperProfiler : Profile
    {
        public MapperProfiler()
        {
            CreateMap<TodoList, DB.TodoList>();
            CreateMap<DB.TodoList, TodoList>();

            CreateMap<TodoListTask, DB.TodoListTask>();
            CreateMap<DB.TodoListTask, TodoListTask>();
        }
    }
}
