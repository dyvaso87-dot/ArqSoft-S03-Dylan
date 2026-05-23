using Catalogo.Application.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddHttpContextAccessor();

// Registro de servicios
builder.Services.AddScoped<UserService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var path = Path.Combine(env.ContentRootPath, "Data", "users.json");
    return new UserService(path);
});

builder.Services.AddScoped<ItemService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var path = Path.Combine(env.ContentRootPath, "Data", "items.json");
    return new ItemService(path);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();