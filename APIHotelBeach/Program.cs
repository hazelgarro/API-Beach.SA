using APIHotelBeach.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Contexto y string de conexion
builder.Services.AddDbContext<APIHotelBeach.Context.DbContextHotel>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("StringConexion")));

//configuracion del servicio JWT
builder.Services.AddScoped<IAutorizacionServices, AutorizacionServices>();

var key = builder.Configuration.GetValue<string>("JwtSettings:Key");
var keyBytes = Encoding.ASCII.GetBytes(key);

builder.Services.AddAuthentication(
    config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(
   config =>
   {
       config.RequireHttpsMetadata = false; 
       config.SaveToken = true; 
       config.TokenValidationParameters = new TokenValidationParameters 
       {
           ValidateIssuerSigningKey = true, 
           IssuerSigningKey = new SymmetricSecurityKey(keyBytes), 
           ValidateIssuer = false, 
           ValidateAudience = false, 
           ValidateLifetime = true, 
           ClockSkew = TimeSpan.Zero 
       };
   });


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
