using Microsoft.EntityFrameworkCore;                 // EF Core
using Recipe_Sharing_Website.Components;             // Blazor root component
using Recipe_Sharing_Website.Data;                   // DbContext & seeder
using Recipe_Sharing_Website.Models;                 // Models

var builder = WebApplication.CreateBuilder(args);

// Enable Razor components (Blazor Server)
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Register DbContextFactory (safe for Blazor Server)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    )
);

// Session support (your session-based auth)
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Register HttpClient directly (NO AddHttpClient, avoids CS1660)
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["AppBaseUrl"] ?? "https://localhost:7170";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

// Enable API controllers (PaymentsController)
builder.Services.AddControllers();

var app = builder.Build();

// Seed DB on startup
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    DbSeeder.Seed(db);
}

// Production error pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session must be enabled before endpoints
app.UseSession();

// Required for EditForm/FormName antiforgery
app.UseAntiforgery();

// ---------- AUTH ENDPOINTS ----------
app.MapPost("/auth/login", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) =>
{
    var form = await ctx.Request.ReadFormAsync();

    var role = (form["Role"].ToString() ?? "User").Trim();
    var username = (form["Username"].ToString() ?? "").Trim();
    var password = (form["Password"].ToString() ?? "");

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.Redirect("/login?err=Missing%20credentials");

    await using var db = await factory.CreateDbContextAsync();

    if (role == "Admin")
    {
        var admin = await db.Admins.FirstOrDefaultAsync(a => a.Username == username);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            return Results.Redirect("/login?err=Invalid%20admin%20credentials");

        ctx.Session.SetString(AuthSession.RoleKey, "Admin");
        ctx.Session.SetString(AuthSession.AdminIdKey, admin.AdminId.ToString());
        ctx.Session.SetString(AuthSession.NameKey, admin.Username);
        ctx.Session.Remove(AuthSession.UserIdKey);
    }
    else
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Results.Redirect("/login?err=Invalid%20user%20credentials");

        ctx.Session.SetString(AuthSession.RoleKey, "User");
        ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
        ctx.Session.SetString(AuthSession.NameKey, user.Username);
        ctx.Session.Remove(AuthSession.AdminIdKey);
    }

    return Results.Redirect("/");
});

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
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    // Auto login after register
    ctx.Session.SetString(AuthSession.RoleKey, "User");
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
    ctx.Session.SetString(AuthSession.NameKey, user.Username);
    ctx.Session.Remove(AuthSession.AdminIdKey);

    return Results.Redirect("/");
});

app.MapPost("/auth/logout", (HttpContext ctx) =>
{
    AuthSession.Logout(ctx.Session);
    return Results.Redirect("/login");
});

// Map API controllers (PaymentsController routes), IF COMMENTED OUT, PAYMENT FUNCTIONALITY IS DISABLED
app.MapControllers();

// Map Blazor components
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
