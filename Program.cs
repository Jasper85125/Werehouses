using item.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
<<<<<<< HEAD
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
=======
builder.Services.AddControllers();

builder.Services.AddTransient<IItemService,ItemService>();

var app = builder.Build();

app.MapControllers();
>>>>>>> af87865afa17a14a1ad12a0cbc15247109763d60

app.UseHttpsRedirection();

app.Run();

