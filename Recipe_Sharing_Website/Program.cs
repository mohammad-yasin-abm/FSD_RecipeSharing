using Microsoft.EntityFrameworkCore;
using Recipe_Sharing_Website.Components;
using Recipe_Sharing_Website.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Blazor components (default for Blazor Web App)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
