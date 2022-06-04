using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SexShop.Data;
using SexShop.Services;

var builder = WebApplication.CreateBuilder(args);

string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL").Remove(0, 11);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = $"Host={GetHost(databaseUrl)};Port=5432;Pooling=True;Database={GetDatabase(databaseUrl)};User Id={GetUser(databaseUrl)};Password={GetPassword(databaseUrl)}";
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<EShopperDbContext>(options => 
    options.UseNpgsql(connectionString));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EShopper}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

static string GetUser(string STR)
{
    string FinalString;
    int Pos2 = STR.IndexOf(":");
    FinalString = STR.Substring(0, Pos2 - 0);
    return FinalString;
}
static string GetPassword(string STR)
{
    string FinalString;
    int Pos1 = STR.IndexOf(":") + ":".Length;
    int Pos2 = STR.IndexOf("@");
    FinalString = STR.Substring(Pos1, Pos2 - Pos1);
    return FinalString;
}
static string GetHost(string STR)
{
    string FinalString;
    int Pos1 = STR.IndexOf("@") + "@".Length;
    int Pos2 = STR.IndexOf(":5432");
    FinalString = STR.Substring(Pos1, Pos2 - Pos1);
    return FinalString;
}
static string GetDatabase(string STR)
{
    string FinalString;
    int Pos1 = STR.IndexOf("5432/") + "5432/".Length;
    FinalString = STR.Substring(Pos1);
    return FinalString;
}