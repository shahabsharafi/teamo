using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using MediatR;
using Teamo.Assets.Email;
using System.Reflection;
using Teamo.Assets.SMS;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Domain;
using Teamo.Identity.API.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services
    .AddIdentityMongoDbProvider<ApplicationUser, MongoRole>(
        identity =>
            {
                identity.Password.RequiredLength = 8;
                // other options
            },
        mongo =>
            {
               mongo.ConnectionString = "mongodb://127.0.0.1:27017/identity";
                // other options
            });

IConfigurationSection authConfig = builder.Configuration.GetSection("Auth");
builder.Services.Configure<IdentitySettings>(authConfig);
//.AddIdentityCore<ApplicationUser>()
//.AddRoles<MongoRole>()
//.AddMongoDbStores<ApplicationUser, MongoRole, ObjectId>(mongo =>
//{
//    mongo.ConnectionString = "mongodb://127.0.0.1:27017/identity";
//    // other options
//})
//.AddDefaultTokenProviders();
builder.Services.AddEmail(builder.Configuration);
builder.Services.AddSMS(builder.Configuration);
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    
    // Register the Swagger generator and the Swagger UI middlewares
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
