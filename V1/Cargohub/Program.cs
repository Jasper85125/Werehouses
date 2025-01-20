using ServicesV1;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseSwagger();
app.UseSwaggerUI();

app.Urls.Add("http://localhost:5001");
app.MapControllers();

app.UseHttpsRedirection();

app.Run();