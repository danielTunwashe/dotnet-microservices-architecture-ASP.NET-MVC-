using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Http client configurations
//Registers the IHTTPContextFactory that we are using in our base class
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

//Then register our coupon service(and other services) to httpclient
builder.Services.AddHttpClient<ICouponService,CouponService>();
builder.Services.AddHttpClient<IAuthService,AuthService>();
builder.Services.AddHttpClient<IProductService,ProductService>();
//Set the value of CouponAPIBase from the URL in the appsetting in the web project from the api launch settings.js
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];


//Register the Base service and coupon service to dependency injection
builder.Services.AddScoped<IBaseService,BaseService>();
builder.Services.AddScoped<ICouponService,CouponService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ITokenProvider,TokenProvider>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        //Not logged In then...
        options.LoginPath = "/Auth/Login";
        //If accessed denied is found then it will go the the action method of accessdenied...
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
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
