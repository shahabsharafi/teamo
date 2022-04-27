using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Teamo.Identity.API.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<MongoRole>()
    .AddMongoDbStores<ApplicationUser, MongoRole, ObjectId>(mongo =>
    {
        mongo.ConnectionString = "mongodb://127.0.0.1:27017/identity";
        // other options
    })
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
