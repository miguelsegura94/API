using BBDD;
using BBDD.Servicios;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar DbContext con la cadena de conexión
/*builder.Services.AddDbContext<AMSEDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BBDD")));*/

// Registrar ServicioBD como un servicio inyectable
builder.Services.AddScoped<ServicioUsuario>(provider =>
    new ServicioUsuario(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ServicioElo>(provider =>
    new ServicioElo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ServicioBD>(provider =>
    new ServicioBD(builder.Configuration.GetConnectionString("DefaultConnection")));

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
