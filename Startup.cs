﻿using Microsoft.AspNetCore.Builder;
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
                .AddDbContext<ApiDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DbApi")))
                .AddTransient<ITodoListService, TodoListService>()
                .AddAutoMapper()
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseErrorHandlingMiddleware(new IExceptionHandler[] {
                new ItemNotFoundExceptionHandler(),
                new ItemExistsExceptionHandler(),
                new UnhandledExceptionHandler()
            });                  
            app.UseMvc();
        }
    }
}