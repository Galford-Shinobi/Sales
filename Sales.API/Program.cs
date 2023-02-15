using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Shared.Applications.Logic;
using Sales.Shared.DataBase;

var builder = WebApplication.CreateBuilder(args);

//Añadimos cache
builder.Services.AddResponseCaching();
// Add services to the container.

//builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddControllers(opcion =>
{
    //Cache profile. Un cache global y así no tener que ponerlo en todas partes
    opcion.CacheProfiles.Add("PorDefecto20Segundos", new CacheProfile() { Duration = 30 });
}).AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SalesDbContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnection"));
});

builder.Services.AddApplication(builder.Configuration);


//builder.Services.AddDbContext<SalesDbContext>(x => x.UseSqlServer("DockerConnection"));
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

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.Run();
