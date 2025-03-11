#region builder
using Microsoft.AspNetCore.Mvc;
using CVProject.Models.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CVProject.Models.Repository.Entity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
#endregion

#region environment variables
var databaseConnStr = builder.Configuration.GetConnectionString("Database");
#endregion

#region services
builder.Services.AddDbContext<CvContext>(options =>
    options.UseSqlServer(databaseConnStr));
builder.Services.AddIdentity<Profile, IdentityRole>().AddEntityFrameworkStores<CvContext>().AddDefaultTokenProviders();

#endregion

#region app
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
#endregion

#region db migraton
// Cant use EF any more since manual DB updates.
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CvContext>();
    context.Database.Migrate();
}*/
#endregion

app.Run();