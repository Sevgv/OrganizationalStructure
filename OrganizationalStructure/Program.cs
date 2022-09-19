using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("OrgStructure"));
connectionStringBuilder.Password = builder.Configuration["DBPassword"];

var connection = connectionStringBuilder.ConnectionString;

builder.Services.AddDbContext<OrgStructureContext>(
    options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(connection)
    );

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
