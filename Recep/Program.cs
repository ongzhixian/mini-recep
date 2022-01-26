using Recep.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

AppStartup.ConfigureWebHost(builder.WebHost);

AppStartup.SetupOptions(builder.Configuration, builder.Services);

AppStartup.SetupAuthentication(builder.Configuration, builder.Services);

AppStartup.SetupAuthorization(builder.Services);

AppStartup.SetupServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Map root to HTTP 204 (no-content) response instead of 404 (not-found)
app.Map("/", (context) =>
{
    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
    return Task.CompletedTask;
});

app.Map("/favicon.ico", (context) =>
{
    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
    return Task.CompletedTask;
});

app.Run();
