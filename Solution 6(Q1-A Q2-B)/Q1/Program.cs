using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

var builder = WebApplication.CreateBuilder(args);

// [GIỮ NGUYÊN] Không sửa đoạn AddControllers bên dưới
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// [SỬA]: Đổi `Prn232PeSu2613Context` -> tên file *Context.cs mới | [XEM Ở ĐÂU]: thư mục Q1/Models/ sau khi chạy lệnh scaffold
builder.Services.AddDbContext<Prn232PeSu2613Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// [GIỮ NGUYÊN] Không sửa đoạn bên dưới
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
