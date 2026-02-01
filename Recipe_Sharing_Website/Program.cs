using Microsoft.EntityFrameworkCore; // EF Core
using Recipe_Sharing_Website.Components; // Blazor root component (App)
using Recipe_Sharing_Website.Data; // DbContext, DbSeeder, AuthSession
using Recipe_Sharing_Website.Models; // Models (User/Admin/etc.)

var builder = WebApplication.CreateBuilder(args); // Create app builder

builder.Services // Begin configuring services
    .AddRazorComponents() // Add Razor Components
    .AddInteractiveServerComponents(); // Enable interactive Blazor Server

builder.Services.AddDbContextFactory<AppDbContext>(options => // Register DbContextFactory (safe for Blazor)
    options.UseSqlServer( // Use SQL Server
        builder.Configuration.GetConnectionString("DefaultConnection"), // Get connection string
        sql => sql.EnableRetryOnFailure() // Retry on transient failures
    )
); // End DbContextFactory registration

builder.Services.AddHttpContextAccessor(); // Allow components to access HttpContext/Session
builder.Services.AddDistributedMemoryCache(); // Needed for session storage
builder.Services.AddSession(options => // Configure session
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Session lifetime
    options.Cookie.HttpOnly = true; // Prevent JS access
    options.Cookie.IsEssential = true; // Required for session to work without consent prompts
}); // End session config

builder.Services.AddScoped(sp => // Register HttpClient for internal API calls from Blazor
{
    var config = sp.GetRequiredService<IConfiguration>(); // Read config
    var baseUrl = config["AppBaseUrl"] ?? "https://localhost:7170"; // Default fallback URL
    return new HttpClient { BaseAddress = new Uri(baseUrl) }; // Create HttpClient with BaseAddress
}); // End HttpClient registration

builder.Services.AddControllers(); // Required for PaymentsController + any other controllers

var app = builder.Build(); // Build the app

using (var scope = app.Services.CreateScope()) // Create scope for seeding
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>(); // Get factory
    await using var db = await factory.CreateDbContextAsync(); // Create db context
    DbSeeder.Seed(db); // Seed initial data
} // End seed scope

if (!app.Environment.IsDevelopment()) // Production-only pipeline
{
    app.UseExceptionHandler("/Error"); // Use error handler page
    app.UseHsts(); // Enable HSTS
} // End production check

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve CSS/JS files

app.UseRouting(); // Enable routing

app.UseSession(); // Enable sessions BEFORE endpoints

app.UseAntiforgery(); // Required because Blazor forms use antiforgery metadata

app.MapPost("/auth/login", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) => // Login endpoint
{
    var form = await ctx.Request.ReadFormAsync(); // Read form data

    var role = (form["Role"].ToString() ?? "User").Trim(); // Role chosen
    var username = (form["Username"].ToString() ?? "").Trim(); // Username
    var password = (form["Password"].ToString() ?? ""); // Password

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) // Validate inputs
        return Results.Redirect("/login?err=Missing%20credentials"); // Redirect with error

    await using var db = await factory.CreateDbContextAsync(); // Create db

    if (role == "Admin") // Admin login path
    {
        var admin = await db.Admins.FirstOrDefaultAsync(a => a.Username == username); // Find admin
        if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash)) // Verify password
            return Results.Redirect("/login?err=Invalid%20admin%20credentials"); // Reject

        ctx.Session.SetString(AuthSession.RoleKey, "Admin"); // Store role
        ctx.Session.SetString(AuthSession.AdminIdKey, admin.AdminId.ToString()); // Store admin id
        ctx.Session.SetString(AuthSession.NameKey, admin.Username); // Store name
        ctx.Session.Remove(AuthSession.UserIdKey); // Clear user id
        AuthSession.SetPremium(ctx.Session, true); // Admin treated as premium for UI simplicity
    }
    else // User login path
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username); // Find user
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) // Verify password
            return Results.Redirect("/login?err=Invalid%20user%20credentials"); // Reject

        ctx.Session.SetString(AuthSession.RoleKey, "User"); // Store role
        ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString()); // Store user id
        ctx.Session.SetString(AuthSession.NameKey, user.Username); // Store name
        ctx.Session.Remove(AuthSession.AdminIdKey); // Clear admin id
        AuthSession.SetPremium(ctx.Session, user.IsPremium); // Store premium flag in session
    }

    return Results.Redirect("/"); // Go home after login
}); // End login endpoint

app.MapPost("/auth/register", async (HttpContext ctx, IDbContextFactory<AppDbContext> factory) => // Register endpoint
{
    var form = await ctx.Request.ReadFormAsync(); // Read form data

    var username = (form["Username"].ToString() ?? "").Trim(); // Username
    var email = (form["Email"].ToString() ?? "").Trim(); // Email
    var password = (form["Password"].ToString() ?? ""); // Password

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) // Validate inputs
        return Results.Redirect("/register?err=Missing%20credentials"); // Reject

    await using var db = await factory.CreateDbContextAsync(); // Create db

    if (await db.Users.AnyAsync(u => u.Username == username)) // Enforce unique username
        return Results.Redirect("/register?err=Username%20already%20exists"); // Reject

    var user = new User // Create new user row
    {
        Username = username, // Store username
        Email = email, // Store email
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), // Hash password
        IsPremium = false // New users start non-premium
    };

    db.Users.Add(user); // Add user
    await db.SaveChangesAsync(); // Save

    ctx.Session.SetString(AuthSession.RoleKey, "User"); // Auto login as user
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString()); // Store user id
    ctx.Session.SetString(AuthSession.NameKey, user.Username); // Store name
    ctx.Session.Remove(AuthSession.AdminIdKey); // Clear admin id
    AuthSession.SetPremium(ctx.Session, false); // Store premium false

    return Results.Redirect("/"); // Go home
}); // End register endpoint

app.MapPost("/auth/logout", (HttpContext ctx) => // Logout endpoint
{
    AuthSession.Logout(ctx.Session); // Clear session
    return Results.Redirect("/login"); // Go login
}); // End logout endpoint

app.MapControllers(); // Map controller routes (PaymentsController, etc.)

app.MapRazorComponents<App>() // Map Blazor app
   .AddInteractiveServerRenderMode(); // Interactive server render mode

app.Run(); // Run the app
