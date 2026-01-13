// EF Core for DB access
using Microsoft.EntityFrameworkCore;

// Blazor components/root host
using Recipe_Sharing_Website.Components;

// App data access + DbSeeder
using Recipe_Sharing_Website.Data;

// Model entities used in auth/register
using Recipe_Sharing_Website.Models;

// Build and configure web application host
var builder = WebApplication.CreateBuilder(args);

// Add support for Razor Components + interactive Blazor Server
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Add support for MVC-style API controllers
builder.Services.AddControllers();

// Configure database context factory with SQL Server connection
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    ));

// Allows capturing HttpContext.Session inside Blazor components
builder.Services.AddHttpContextAccessor();

// In-memory store used by Session
builder.Services.AddDistributedMemoryCache();

// Configure session storage (stored in memory, not cookies)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the web application host
var app = builder.Build();

// Create database scope and seed demo user/admin accounts
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    DbSeeder.Seed(db);
}

// Add exception handling + HSTS for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Force HTTPS for all traffic
app.UseHttpsRedirection();

// Enable static files (CSS, JS, images)
app.UseStaticFiles();

// Enable routing middleware
app.UseRouting();

// Enable session middleware for auth state
app.UseSession();

// Required for EditForm + FormName POST protections
app.UseAntiforgery();

// Maps all controllers (e.g., PaymentsController)
app.MapControllers();

// Handle POST /auth/login for both Admin + User logins
app.MapPost("/auth/login", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) =>
{
    var form = await ctx.Request.ReadFormAsync();

    var role = (form["Role"].ToString() ?? "User").Trim();
    var username = (form["Username"].ToString() ?? "").Trim();
    var password = (form["Password"].ToString() ?? "");

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.Redirect("/login?err=Missing%20credentials");

    await using var db = await factory.CreateDbContextAsync();

    // Admin login path
    if (role == "Admin")
    {
        var admin = await db.Admins.FirstOrDefaultAsync(a => a.Username == username);
        if (admin is null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            return Results.Redirect("/login?err=Invalid%20admin%20credentials");

        ctx.Session.SetString(AuthSession.RoleKey, "Admin");
        ctx.Session.SetString(AuthSession.AdminIdKey, admin.AdminId.ToString());
        ctx.Session.SetString(AuthSession.NameKey, admin.Username);
        ctx.Session.SetString(AuthSession.PremiumKey, "false");
        ctx.Session.Remove(AuthSession.UserIdKey);

        return Results.Redirect("/");
    }

    // User login path
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        return Results.Redirect("/login?err=Invalid%20user%20credentials");

    ctx.Session.SetString(AuthSession.RoleKey, "User");
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
    ctx.Session.SetString(AuthSession.NameKey, user.Username);
    ctx.Session.SetString(AuthSession.PremiumKey, user.IsPremium ? "true" : "false");
    ctx.Session.Remove(AuthSession.AdminIdKey);

    return Results.Redirect("/");
});

// Handle POST /auth/register for user self-registration
app.MapPost("/auth/register", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) =>
{
    var form = await ctx.Request.ReadFormAsync();

    var username = (form["Username"].ToString() ?? "").Trim();
    var email = (form["Email"].ToString() ?? "").Trim();
    var password = (form["Password"].ToString() ?? "");

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.Redirect("/register?err=Missing%20credentials");

    await using var db = await factory.CreateDbContextAsync();

    if (await db.Users.AnyAsync(u => u.Username == username))
        return Results.Redirect("/register?err=Username%20already%20exists");

    var user = new User
    {
        Username = username,
        Email = email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        IsPremium = false
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    ctx.Session.SetString(AuthSession.RoleKey, "User");
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
    ctx.Session.SetString(AuthSession.NameKey, user.Username);
    ctx.Session.SetString(AuthSession.PremiumKey, "false");
    ctx.Session.Remove(AuthSession.AdminIdKey);

    return Results.Redirect("/");
});

// Handle POST /auth/logout and clear all session info
app.MapPost("/auth/logout", (HttpContext ctx) =>
{
    AuthSession.Logout(ctx.Session);
    return Results.Redirect("/login");
});

// Map Blazor components + enable interactive features
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Start request pipeline + listen for incoming connections
app.Run();
