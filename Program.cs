var builder = WebApplication.CreateBuilder(args);

#if (RELEASE)
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8081"; // Default to 8080 if PORT is not set
    builder.WebHost.UseUrls($"http://*:{port}");
#endif


// Add services to the container.
builder.Services.AddSingleton<SnowflakeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Remove HTTPS redirection when running on Railway
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT")))
{
    Console.WriteLine("Running on Railway - Skipping HTTPS Redirection");
}
else
{
    app.UseHttpsRedirection(); // Enable locally
}

app.MapGet("/", () => "Hello from Railway!");

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
