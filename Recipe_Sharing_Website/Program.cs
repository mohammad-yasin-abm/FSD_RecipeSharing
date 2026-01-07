using Microsoft.EntityFrameworkCore;
using Recipe_Sharing_Website.Components;
using Recipe_Sharing_Website.Data;
using Recipe_Sharing_Website.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    ));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// If your project already uses authentication middleware, keep these.
// If not configured, you can delete ONLY these two lines.
app.UseAuthentication();
app.UseAuthorization();

// ✅ This fixes your current crash (because "/" has antiforgery metadata)
app.UseAntiforgery();

app.MapControllers();

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
        if (admin is null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            return Results.Redirect("/login?err=Invalid%20admin%20credentials");

        ctx.Session.SetString(AuthSession.RoleKey, "Admin");
        ctx.Session.SetString(AuthSession.AdminIdKey, admin.AdminId.ToString());
        ctx.Session.SetString(AuthSession.NameKey, admin.Username);
        ctx.Session.Remove(AuthSession.UserIdKey);
    }
    else
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Results.Redirect("/login?err=Invalid%20user%20credentials");

        ctx.Session.SetString(AuthSession.RoleKey, "User");
        ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
        ctx.Session.SetString(AuthSession.NameKey, user.Username);
        ctx.Session.Remove(AuthSession.AdminIdKey);
    }

    return Results.Redirect("/");
})
.DisableAntiforgery(); // ✅ IMPORTANT: prevents antiforgery from breaking login

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

    ctx.Session.SetString(AuthSession.RoleKey, "User");
    ctx.Session.SetString(AuthSession.UserIdKey, user.UserId.ToString());
    ctx.Session.SetString(AuthSession.NameKey, user.Username);
    ctx.Session.Remove(AuthSession.AdminIdKey);

    return Results.Redirect("/");
})
.DisableAntiforgery(); // ✅ IMPORTANT: prevents antiforgery from breaking register

app.MapPost("/auth/logout", (HttpContext ctx) =>
{
    AuthSession.Logout(ctx.Session);
    return Results.Redirect("/login");
})
.DisableAntiforgery(); // ✅ IMPORTANT: prevents antiforgery from breaking logout

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
