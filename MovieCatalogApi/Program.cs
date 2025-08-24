using Microsoft.EntityFrameworkCore;
using MovieCatalogApi.Data;

var builder = WebApplication.CreateBuilder(args);

bool useSqlServer = builder.Configuration.GetValue<bool>("UseSqlServer", true);

if (useSqlServer)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    // Optional fallback to InMemory for quick tests
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("MovieCatalogDb"));
}

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (useSqlServer)
    {
        db.Database.Migrate(); // applies migrations automatically on startup
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
