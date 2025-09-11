using Mango.Services.OrderAPI.Application;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Extensions;
using Mango.Services.OrderAPI.Service;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var applicationAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddScoped<IProductService, ProductService>();


//This helps when we are calling another endpoint within our service and we want to pass the access token
//This helps us to access the HttpContext inside our HttpClientHandler class
builder.Services.AddHttpContextAccessor();
//We are adding a custom httpclient handler to append the access token to the request
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

//Now we need to add an httpclient for our product api in program.cs
//for product
//We also need to pass the AddHttpMessageHandler if we want to complete the passing of the 
//Bearer token we pass to the other api been called..
builder.Services.AddHttpClient("Product", u => u.BaseAddress =
 new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//We are configuring the swaggerGen to use authentication in our Api
//Things needed by default when you are adding authentication to your swagger Defination
//it has to do with all we need for authentication in the swaggerUI
builder.Services.AddSwaggerGen(option =>
{

    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});

//Register MediatR and Automapper
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(applicationAssembly));

builder.Services.AddAutoMapper(typeof(OrderProfile));


//This has to do with all we need for authentication
builder.AddAppAuthentication();

//Registers the authorization middleware so [Authorize] actually works once a user is authenticated.
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigrations();

app.Run();

void ApplyMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbContext.Database.GetPendingMigrations().Count() > 0)
        {
            dbContext.Database.Migrate();
        }

    }
}


