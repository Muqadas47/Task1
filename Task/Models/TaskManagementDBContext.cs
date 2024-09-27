using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Task;

public class TaskManagementDBContext : DbContext
{
    public TaskManagementDBContext() : base("name=TaskManagementDBContext")
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskView> TaskViews { get; set; }

    public DbSet<AssignTasks> AssignTasks { get; set; }

    public DbSet<TaskActivities> TaskActivities { get; set; }

    public DbSet<SystemSettings> SystemSettings { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Define primary keys
        

        base.OnModelCreating(modelBuilder);
    }
}

public class User 
{
    public int UserID { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
   
    public DateTime? SignupDate { get; internal set; }
    public bool IsBlocked { get; internal set; }
    public bool IsDeleted { get; internal set; }
    
}


public class TaskView
{
    [Key]
    public int TaskID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string AssignedTo { get; set; }
    public DateTime DueDate { get; set; }
    
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}
public partial class AssignTasks
{
    [Key]
    public int AssignTaskID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Nullable<System.DateTime> Deadline { get; set; }
    public string Priority { get; set; }
    public string Category { get; set; }
    public Nullable<int> AssignedUserID { get; set; }
    public string CreatedBy { get; set; }

    public string Status { get; set; }
    
}



    public partial class TaskActivities
    {
        [Key]

        public int ActivityId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string ActivityDescription { get; set; }
        public string TaskStatus { get; set; }
        public Nullable<int> ProgressPercentage { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        
    }
public class TaskCommentsViewModel
{
    public int TaskID { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string Username { get; set; }
    public string Comments { get; set; }
}

