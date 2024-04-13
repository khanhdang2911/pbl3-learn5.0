using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;
using PBL3_Course.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
        {
            string connectString = builder.Configuration.GetConnectionString("MyBlogContext");
            options.UseSqlServer(connectString);
        });
builder.WebHost.ConfigureKestrel(options=>{
    options.Limits.MaxRequestBodySize=512*1024*1024;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(50); // Thời gian chờ mặc định là 10 phút

});
builder.Services.AddSingleton<HashPasswordByBC>();
builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 512*1024*1024; // Dung lượng tối đa của toàn bộ request
    });
//Cookie login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath="/User/Login";
        // options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        // options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Login/Forbidden/";
    });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IVnPayServices, VnPayService>();

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
