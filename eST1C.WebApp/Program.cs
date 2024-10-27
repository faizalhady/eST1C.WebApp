using eST1C.WebApp.Components;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;
using eST1C.WebApp.Service;
using Radzen.Blazor;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

    builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<LogDataService>();
builder.Services.AddScoped<PCNameCountService>();
builder.Services.AddScoped<LogServices>();


var app = builder.Build();

Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("DefaultConnection")}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();

    // Fetch some data and log it to the console
    // var ValidLogs = dbContext.ValidLogs.Take(5).ToList(); // Fetch 5 rows to check
    // foreach (var log in ValidLogs)
    // {
    //     // Console.WriteLine($"PC Name: {log.PCName}, Timestamp: {log.Timestamp}");
    // }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
