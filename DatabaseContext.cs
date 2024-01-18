using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskListServer;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskList> TaskLists { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=Database.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");
        });

        modelBuilder.Entity<TaskList>(entity =>
        {
            entity.ToTable("TaskList");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskLists).HasForeignKey(d => d.TaskId);

            entity.HasOne(d => d.User).WithMany(p => p.TaskLists).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
