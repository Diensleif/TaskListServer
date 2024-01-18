using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace TaskListServer;

public partial class Task
{
    public long Id { get; set; }

    public string? Date { get; set; }

    public string? Description { get; set; }

    public long? Status { get; set; }

    public virtual ICollection<TaskList> TaskLists { get; } = new List<TaskList>();
}
