using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Http client configurations
//Registers the IHTTPContextFactory that we are using in our base class
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
//Then register our coupon service via httpclient
builder.Services.AddHttpClient<ICouponService,CouponService>();
//Set the value of CouponAPIBase from the URL in the appsetting in the web project from the api launch settings.js
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];


//Register the Base service and coupon service to dependency injection
builder.Services.AddScoped<IBaseService,BaseService>();
builder.Services.AddScoped<ICouponService,CouponService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
