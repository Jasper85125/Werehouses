using ServicesV1;
//using ApiKeyAuthentication.Authentication;


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
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IShipmentService, ShipmentService>();
builder.Services.AddTransient<ISupplierService, SupplierService>();
builder.Services.AddTransient<ITransferService, TransferService>();
builder.Services.AddTransient<IOrderService, OrderService>();

var app = builder.Build();
app.Urls.Add("http://localhost:5001");
app.MapControllers();

app.UseHttpsRedirection();

app.Run();