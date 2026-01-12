using Microsoft.EntityFrameworkCore; // EF Core
using Recipe_Sharing_Website.Components; // Blazor root component
using Recipe_Sharing_Website.Data; // AppDbContext, DbSeeder, AuthSession
using Recipe_Sharing_Website.Models; // User model

var builder = WebApplication.CreateBuilder(args); // Create the web app builder

builder.Services.AddRazorComponents() // Add Razor components services
    .AddInteractiveServerComponents(); // Enable Interactive Server rendering

builder.Services.AddControllers(); // Enable API controllers (/api/*)

builder.Services.AddDbContextFactory<AppDbContext>(options => // Register DbContextFactory (safe for Blazor Server)
    options.UseSqlServer( // Use SQL Server provider
        builder.Configuration.GetConnectionString("DefaultConnection"), // Get connection string from appsettings.json
        sql => sql.EnableRetryOnFailure() // Retry transient SQL failures
    ));

builder.Services.AddHttpContextAccessor(); // Allow injecting IHttpContextAccessor
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options => // Configure session
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Session expiry time
    options.Cookie.HttpOnly = true; // Cookie not accessible by JS
    options.Cookie.IsEssential = true; // Cookie always enabled (even without consent)
});

var app = builder.Build(); // Build the app

using (var scope = app.Services.CreateScope()) // Create DI scope for seeding
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>(); // Resolve DbContextFactory
    await using var db = await factory.CreateDbContextAsync(); // Create DbContext instance
    DbSeeder.Seed(db); // Seed demo data
}

if (!app.Environment.IsDevelopment()) // If not development environment
{
    app.UseExceptionHandler("/Error"); // Use error page
    app.UseHsts(); // Enable HSTS
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve wwwroot files

app.UseRouting(); // Enable endpoint routing
app.Use(async (ctx, next) =>
{
    Console.WriteLine($"REQ => {ctx.Request.Method} {ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Request.Path}");
    await next();
});


app.UseSession(); // Enable session middleware

app.UseAuthorization(); // Authorization middleware (safe even without Identity)

app.UseAntiforgery(); // Required because EditForm + FormName adds antiforgery metadata

app.MapPost("/auth/login", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) => // Login endpoint
{
    var form = await ctx.Request.ReadFormAsync(); // Read posted form fields

    var role = (form["Role"].ToString() ?? "User").Trim(); // Read role selection
    var username = (form["Username"].ToString() ?? "").Trim(); // Read username
    var password = (form["Password"].ToString() ?? ""); // Read password

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) // Validate inputs
        return Results.Redirect("/login?err=Missing%20credentials"); // Redirect with error

    await using var db = await factory.CreateDbContextAsync(); // Create DbContext

    if (role == "Admin") // Admin login flow
    {
        var admin = await db.Admins.FirstOrDefaultAsync(a => a.Username == username); // Find admin by username
        if (admin is null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash)) // Verify password hash
            return Results.Redirect("/login?err=Invalid%20admin%20credentials"); // Redirect if invalid

        ctx.Session.SetString(AuthSession.RoleKey, "Admin"); // Store role in session
        ctx.Session.SetString(AuthSession.AdminIdKey, admin.AdminId.ToString()); // Store admin id in session
        ctx.Session.SetString(AuthSession.NameKey, admin.Username); // Store display name in session
        ctx.Session.Remove(AuthSession.UserIdKey); // Remove user id if present
    }
    else // User login flow
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username); // Find user by username
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Verify password hash
            return Results.Redirect("/login?err=Invalid%20user%20credentials"); // Redirect if invalid

        ctx.Session.SetString(AuthSession.RoleKey, "User"); // Store role in session
        ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString()); // Store user id in session
        ctx.Session.SetString(AuthSession.NameKey, user.Username); // Store display name in session
        ctx.Session.Remove(AuthSession.AdminIdKey); // Remove admin id if present
    }

    return Results.Redirect("/"); // Redirect to home after login
});

app.MapPost("/auth/register", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) => // Register endpoint
{
    var form = await ctx.Request.ReadFormAsync(); // Read posted form fields

    var username = (form["Username"].ToString() ?? "").Trim(); // Read username
    var email = (form["Email"].ToString() ?? "").Trim(); // Read email
    var password = (form["Password"].ToString() ?? ""); // Read password

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) // Validate inputs
        return Results.Redirect("/register?err=Missing%20credentials"); // Redirect with error

    await using var db = await factory.CreateDbContextAsync(); // Create DbContext

    if (await db.Users.AnyAsync(u => u.Username == username)) // Enforce unique username
        return Results.Redirect("/register?err=Username%20already%20exists"); // Redirect with error

    var user = new User // Create new user entity
    {
        Username = username, // Set username
        Email = email, // Set email
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), // Hash password
        IsPremium = false // Default premium status
    };

    db.Users.Add(user); // Add to database
    await db.SaveChangesAsync(); // Save changes to assign UserId

    ctx.Session.SetString(AuthSession.RoleKey, "User"); // Auto-login role
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString()); // Auto-login user id
    ctx.Session.SetString(AuthSession.NameKey, user.Username); // Auto-login display name
    ctx.Session.Remove(AuthSession.AdminIdKey); // Remove admin id if present

    return Results.Redirect("/"); // Redirect to home after register
});

app.MapPost("/auth/logout", (HttpContext ctx) => // Logout endpoint
{
    AuthSession.Logout(ctx.Session); // Clear session
    return Results.Redirect("/login"); // Redirect to login page
});

app.MapControllers(); // Map API controllers (PaymentsController etc.)

app.MapRazorComponents<App>() // Map Blazor app endpoints
   .AddInteractiveServerRenderMode(); // Enable interactive server render mode

app.Run(); // Run the app
