using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoListApi.Storage;
using TodoListApi.Services;
using AutoMapper;
using TodoListApi.ExceptionHandling;
using TodoListApi.ExceptionHandling.Handlers;

namespace TodoListApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services                
                .AddSingleton<IStorageContext, StorageContext>()
                .AddTransient<ITodoListService, TodoListService>()
                .AddTransient<ITodoListTasksService, TodoListTasksService>()
                .AddAutoMapper()
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseErrorHandlingMiddleware(new IExceptionHandler[] {
                new DataAccessExceptionHandlers(),
                new UnhandledExceptionHandler()
            });                  
            app.UseMvc();
        }
    }
}
