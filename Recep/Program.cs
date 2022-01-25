using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Not all KestrelServerOptions can be configured from appsettings.json
    // Unfortunately, AddServerHeader is one of them.
    // The rationale is that KestrelServerOptions configurable from appsettings.json
    // are settings that makes more sense to be configured during runtime.
    options.AddServerHeader = false;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Map root to HTTP 204 (no-content) response instead of 404 (not-found)
app.Map("/", (context) =>
{
    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
    return Task.CompletedTask;
});


app.Run();
