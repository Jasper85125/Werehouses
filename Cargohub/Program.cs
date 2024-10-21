using item.Services;
using itemtype.Services;
using warehouse.Services;
using inventory.Services;
using Services;
using itemgroup.Services;
using itemlines.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddTransient<IItemService,ItemService>();
builder.Services.AddTransient<IWarehouseService, WarehouseService>();
builder.Services.AddTransient<IItemtypeService, ItemTypeService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IitemGroupService, ItemGroupService>(); 
builder.Services.AddTransient<IItemLineService, ItemLineService>(); 
builder.Services.AddTransient<ILocationService, LocationService>();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();