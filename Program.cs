using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BakeryContext>(options =>
    options.UseSqlite("Data Source=bageri.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BakeryContext>();
    context.Database.EnsureCreated();
    DbSeeder.Seed(context);
}

app.MapControllers();
app.Run();
