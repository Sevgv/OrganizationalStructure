using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Infrastructure;
using OrganizationalStructure.Infrastructure.Repositories;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

//TODO: ���������������� ��� �������, ������ � ����������
//TODO: ������� �����������
//TODO: ������� ��������� ����� Fluent Validation
//TODO: ������� ��������� ����������
//TODO: ������� ���� �����

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("OrgStructure"))
{
    Password = builder.Configuration["DBPassword"]
};

var connection = connectionStringBuilder.ConnectionString;

builder.Services.AddDbContext<OrgStructureContext>(
    options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(connection)
    );

// Add repository services
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IOrgStructureRepository, OrgStructureRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
