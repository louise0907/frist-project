using Ecomwed.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using static Ecomwed.Models.Cappdbcontext;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Add Library Class Connect to Database
builder.Services.AddDbContext<Cappdbcontext>                                    //Same with appsettings.json
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserRespository")));
// / Add Library Class Connect to Database


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<scriptsecrets>(builder.Configuration.GetSection("Stripe"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=custermor}/{action=login}/{id?}");

app.Run();
