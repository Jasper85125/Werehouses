using item.Services;
using warehouse.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddTransient<IItemService,ItemService>();
builder.Services.AddTransient<IWarehouseService, WarehouseService>();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
