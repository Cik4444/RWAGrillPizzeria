using Microsoft.EntityFrameworkCore;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add DbContext with SQL Server connection
builder.Services.AddDbContext<RwagrillContext>(options => {
    options.UseSqlServer("name=ConnectionStrings:DbConnStr");
});

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Korisnik/Login"; // Redirect to Login when not authenticated
        options.LogoutPath = "/Korisnik/Logout"; // Redirect after logout
        options.AccessDeniedPath = "/Korisnik/Login"; // Redirect when access is denied

        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set expiration time
        options.SlidingExpiration = true; // Reset expiration on activity
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Ensure HTTPS in production
}

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
