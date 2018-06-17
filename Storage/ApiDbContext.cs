using Microsoft.EntityFrameworkCore;
using TodoListApi.Storage.Entities;

namespace TodoListApi.Storage
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoListTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var todoList = modelBuilder.Entity<TodoList>();

            todoList.Property(e => e.Id)
                .ValueGeneratedNever();

            todoList.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            todoList.Property(e => e.Description)
                .HasMaxLength(255);

            todoList.HasMany(e => e.Tasks)
                .WithOne(e => e.TodoList)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            var todoListTask = modelBuilder.Entity<TodoListTask>();

            todoListTask.Property(e => e.Id)
                .ValueGeneratedNever();

            todoListTask.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            todoListTask.Property(e => e.Completed)
                .IsRequired();
        }
    }
}