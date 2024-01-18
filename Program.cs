using Microsoft.AspNetCore.Mvc;

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
                else
                {
                    return Results.NotFound();
                }
            });

            app.MapPost("/registration", ([FromBody] User user) =>
            {
                if (databaseContext.Users.Any(x => x.Login == user.Login))
                {
                    return Results.Conflict();
                }
                else
                {
                    databaseContext.Users.Add(user);
                    databaseContext.SaveChanges();
                    return Results.Ok();
                }
            });

            app.Run();
        }
    }
}
