using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static TaskListServer.Program;

namespace TaskListServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            DatabaseContext databaseContext = new DatabaseContext();

            app.MapPost("/authorization", ([FromBody] User user) =>
            {
                if (databaseContext.Users.Any(x => x.Login == user.Login && x.Password == user.Password))
                {
                    return Results.Ok();
                }

                return Results.NotFound();
            });

            app.MapPost("/registration", ([FromBody] User user) =>
            {
                if (string.IsNullOrEmpty(user.Login))
                {
                    return Results.BadRequest();
                }

                if (databaseContext.Users.Any(x => x.Login == user.Login))
                {
                    return Results.Conflict();
                }

                if (string.IsNullOrEmpty(user.Password))
                {
                    return Results.BadRequest();
                }

                databaseContext.Users.Add(user);
                databaseContext.SaveChanges();
                return Results.Ok();
            });

            const string headerTemplate =
                @"<input class=""text-box"" type=""text"" placeholder=""Task"" id=""description-input""/>" +
                @"<input class=""button"" type =""button"" value =""Добавить"" onclick =""addTask();""/>";

            app.MapPost("/task-list", ([FromBody] User clientUser) =>
            {
                User? user = databaseContext.Users.FirstOrDefault(x => x.Login == clientUser.Login && x.Password == clientUser.Password);

                if (user != null)
                {
                    string response = headerTemplate;

                    List<TaskList> taskLists = databaseContext.TaskLists.Where(x => x.UserId == user.Id).ToList();
                    foreach (TaskList taskList in taskLists)
                    {
                        Task bufferedTtask = databaseContext.Tasks.FirstOrDefault(x => x.Id == taskList.Id)!;
                        response += GetTaskHtml(bufferedTtask);
                    }

                    return Results.Text(response);
                }

                return Results.NotFound();
            });

            app.MapPost("/add-task", ([FromBody] AddTaskJson addTaskJson) =>
            {
                User? user = databaseContext.Users.FirstOrDefault(x => x.Login == addTaskJson.Login && x.Password == addTaskJson.Password);

                if (user != null)
                {
                    Task task = new Task();
                    task.Date = DateTime.Now.ToShortDateString();
                    task.Description = addTaskJson.Description;
                    task.Status = 0;

                    databaseContext.Tasks.Add(task);
                    databaseContext.SaveChanges();

                    TaskList taskListForAdd = new TaskList();
                    taskListForAdd.UserId = user.Id;
                    taskListForAdd.TaskId = task.Id;
                    databaseContext.TaskLists.Add(taskListForAdd);
                    databaseContext.SaveChanges();

                    string response = headerTemplate;

                    List<TaskList> taskLists = databaseContext.TaskLists.Where(x => x.UserId == user.Id).ToList();
                    foreach (TaskList taskList in taskLists)
                    {
                        Task bufferedTtask = databaseContext.Tasks.FirstOrDefault(x => x.Id == taskList.Id)!;
                        response += GetTaskHtml(bufferedTtask);
                    }

                    return Results.Text(response);
                }

                return Results.NotFound();
            });

            app.MapPost("/change-status", ([FromBody] ChangeStatusJson changeStatusJson) =>
            {
                User? user = databaseContext.Users.FirstOrDefault(x => x.Login == changeStatusJson.Login && x.Password == changeStatusJson.Password);

                if (user != null)
                {
                    Task? task = databaseContext.Tasks.FirstOrDefault(x => x.Id == changeStatusJson.TaskId);

                    if (task != null)
                    {

                        if (task.Status == 0)
                        {
                            task.Status = 1;
                        }
                        else
                        {
                            task.Status = 0;
                        }
                        databaseContext.SaveChanges();

                        string response = headerTemplate;

                        List<TaskList> taskLists = databaseContext.TaskLists.Where(x => x.UserId == user.Id).ToList();
                        foreach (TaskList taskList in taskLists)
                        {
                            Task bufferedTtask = databaseContext.Tasks.FirstOrDefault(x => x.Id == taskList.Id)!;
                            response += GetTaskHtml(bufferedTtask);
                        }

                        return Results.Text(response);
                    }
                    else
                    {
                        return Results.BadRequest();
                    }
                }

                return Results.NotFound();
            });

            app.MapPost("/delete-task", ([FromBody] DeleteTaskJson deleteTaskJson) => {
                User? user = databaseContext.Users.FirstOrDefault(x => x.Login == deleteTaskJson.Login && x.Password == deleteTaskJson.Password);

                if (user != null)
                {
                    Task? task = databaseContext.Tasks.FirstOrDefault(x => x.Id == deleteTaskJson.TaskId);

                    if (task != null)
                    {
                        TaskList? taskListForDelete = databaseContext.TaskLists.FirstOrDefault(x => x.TaskId == deleteTaskJson.TaskId);
                        if (taskListForDelete != null)
                        {
                            databaseContext.TaskLists.Remove(taskListForDelete);
                            databaseContext.SaveChanges();
                        }

                        databaseContext.Tasks.Remove(task);
                        databaseContext.SaveChanges();

                        string response = headerTemplate;

                        List<TaskList> taskLists = databaseContext.TaskLists.Where(x => x.UserId == user.Id).ToList();
                        foreach (TaskList taskList in taskLists)
                        {
                            Task bufferedTtask = databaseContext.Tasks.FirstOrDefault(x => x.Id == taskList.Id)!;
                            response += GetTaskHtml(bufferedTtask);
                        }

                        return Results.Text(response);
                    }
                    else
                    {
                        return Results.BadRequest();
                    }
                }

                return Results.NotFound();
            });

            app.Run();
        }

        public static string GetTaskHtml(Task task)
        {
            string status = string.Empty;
            if (task.Status == 1)
            {
                status = "checked";
            }

            return
                "<div class=\"task\">" +
                $"<label class=\"date\">{task.Date}</label>" +
                $"<input class=\"status\" type=\"checkbox\" {status} onclick=\"changeStatus({task.Id});\">" +
                $"<label class=\"description\">{task.Description}</label>" +
                $"<img class=\"image-button\" src=\"trash.svg\" onclick=\"deleteTask({task.Id});\">" +
                "</div>";
        }

        public class AddTaskJson
        {
            public string? Login { get; set; }
            public string? Password { get; set; }
            public string? Description { get; set; }
        }

        public class ChangeStatusJson
        {
            public string? Login { get; set; }
            public string? Password { get; set; }
            public long? TaskId { get; set; }
        }

        public class DeleteTaskJson
        {
            public string? Login { get; set; }
            public string? Password { get; set; }
            public long? TaskId { get; set; }
        }
    }
}
