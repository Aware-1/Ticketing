using Data.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QodraTick.Components;
using QodraTick.Hubs;
using Service.IService;
using Service.Service;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Razor Components (Required for .NET 9)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ✅ Add Controllers support
builder.Services.AddControllers();

// ✅ Add HttpClient for calling APIs
builder.Services.AddHttpClient();

// Entity Framework with Warning Suppression
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Suppress the PendingModelChangesWarning
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// ✅ Configure Antiforgery with exemptions
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__RequestVerificationToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "QodraTickAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Events for handling authentication redirects
        options.Events.OnRedirectToLogin = context =>
        {
            // If this is an API request, return 401 instead of redirecting
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("SupportOnly", policy => policy.RequireRole("Support", "Admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// CORS for SignalR (if needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7295", "http://localhost:5148")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ UseAntiforgery AFTER UseStaticFiles and BEFORE UseAuthentication
app.UseAntiforgery();

// CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors("SignalRCorsPolicy");
}

app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers for API endpoints
app.MapControllers();

// ✅ Configure Render Modes - Updated for .NET 9
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map SignalR Hub
app.MapHub<TicketHub>("/ticketHub");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Apply pending migrations
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
            logger.LogInformation("Migration ها با موفقیت اجرا شدند");
        }
        else
        {
            logger.LogInformation("هیچ migration معلقی وجود ندارد");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "خطا در اجرای migration ها");

        // In development, try to create database if it doesn't exist
        if (app.Environment.IsDevelopment())
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                logger.LogInformation("دیتابیس جدید ایجاد شد");
            }
            catch (Exception createEx)
            {
                logger.LogError(createEx, "خطا در ایجاد دیتابیس");
                throw;
            }
        }
        else
        {
            throw;
        }
    }
}

app.Run();