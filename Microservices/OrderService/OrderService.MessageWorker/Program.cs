using Microsoft.EntityFrameworkCore;
using OrderService.Data.Context;
using OrderService.MessageWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Singleton);

var host = builder.Build();
host.Run();
