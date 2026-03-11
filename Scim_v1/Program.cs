using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scim_v1.Context;
using Scim_v1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    var hasher = new PasswordHasher<User>();

    if (!context.Users.Any(u => u.UserName == "admin"))
    {
        User user = new User();
        user.UserName = "admin";
        user.FirstName = "Elif";
        user.LastName = "Çimen";
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = hasher.HashPassword(user, "123456");

        context.Users.Add(user);
        context.SaveChanges();
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
