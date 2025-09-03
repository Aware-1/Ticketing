using Data.Context;
using Microsoft.EntityFrameworkCore;
using QodraTick.Components;
using Service.IService;
using Service.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Active Directory Authentication
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SupportOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Support") || context.User.IsInRole("Admin")));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin")));
});

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();

// SignalR
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
