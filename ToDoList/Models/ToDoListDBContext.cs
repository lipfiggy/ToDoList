using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models
{
    public sealed class ToDoListDBContext: DbContext
    {
        public ToDoListDBContext(DbContextOptions<ToDoListDBContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        internal DbSet<TaskItem> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                        .Property(x => x.Done)
                        .HasDefaultValue(false);
        }
    }
}
