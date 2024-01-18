using System;
using System.Collections.Generic;

namespace TaskListServer;

public partial class TaskList
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? TaskId { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? User { get; set; }
}
