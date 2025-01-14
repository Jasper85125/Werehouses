using ServicesV2;
using ApiKeyAuthentication.Authentication;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "My API V2",
        Version = "v2",
        Description = "API documentation for version 2"
    });
});
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
builder.Services.AddTransient<IAdminService, AdminService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
});

app.UseMiddleware<ApiKeyMiddleware>();

app.Urls.Add("http://localhost:5002");
app.MapControllers();

app.UseHttpsRedirection();

app.Run();