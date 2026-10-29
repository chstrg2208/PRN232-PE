using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers + JSON bo qua vong lap (Trainer -> Bookings -> Trainer ...)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 2. DbContext doc connection string "MyCnn" tu appsettings.json
builder.Services.AddDbContext<Prn232PeSu2613Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// 3. Swagger de test
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
