using item.Services;
using itemtype.Services;
using warehouse.Services;
using inventory.Services;
using itemgroup.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddTransient<IItemService,ItemService>();
builder.Services.AddTransient<IWarehouseService, WarehouseService>();
builder.Services.AddTransient<IItemtypeService, ItemTypeService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IitemgroupService, ItemgroupService>(); 


var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();