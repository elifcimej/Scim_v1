using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scim_v1.Context;
using Scim_v1.Models;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "Token",
        In = ParameterLocation.Header,
        Description = "Bearer token giriniz"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
}); 
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<ScimAuthMiddleware>();

var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
Console.WriteLine(token);

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
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
