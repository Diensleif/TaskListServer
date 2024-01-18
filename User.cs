using System;
using System.Collections.Generic;

namespace TaskListServer;

public partial class User
{
    public long Id { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<TaskList> TaskLists { get; } = new List<TaskList>();
}
