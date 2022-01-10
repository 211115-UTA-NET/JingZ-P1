using StoreApi.DataStorage;

// connection to db
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// for local testing:
// string connectionString = File.ReadAllText(builder.Configuration.GetConnectionString("Store-DB-Connection"));
// for azure app service:
string connectionString = builder.Configuration.GetConnectionString("Store-DB-Connection");

IRepository repository = new SqlRepository(connectionString);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// provide any other dependencies that, e.g., the controllers need injected into them
// "if anyone asks for an IRepository, give them this object"
builder.Services.AddSingleton<IRepository>(repository);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
