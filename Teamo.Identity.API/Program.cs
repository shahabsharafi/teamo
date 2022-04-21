using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository;
using Teamo.Identity.API.Infrastructure;
using Teamo.Identity.API.Infrastructure.Data;
using Teamo.Identity.API.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MongoSettings>()
    .Bind(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddMongoDBIdentity();

// Add services to the container.

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
