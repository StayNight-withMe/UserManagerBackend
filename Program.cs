using WebApplication1.DI;
using WebApplication1.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WebApplication1.Persistence.Context.ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseInfrastructure();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

