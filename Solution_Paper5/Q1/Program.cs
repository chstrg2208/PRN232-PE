using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// STEP 1: ĐĂNG KÝ CONTROLLER + CHỐNG LỖI VÒNG LẶP JSON (BẮT BUỘC GIỮ NGUYÊN)
// Giải thích: Khi trả về bảng có quan hệ 1-N hoặc N-N (VD: Trainer -> Bookings -> Trainer),
// nếu không có ReferenceHandler.IgnoreCycles sẽ bị lỗi 500 JSON cycle.
// ⚠️ [NẾU ĐỔI ĐỀ]: GIỮ NGUYÊN 100% ĐOẠN NÀY, KHÔNG CẦN SỬA GÌ!
// =====================================================================
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// =====================================================================
// STEP 2: ĐĂNG KÝ DBCONTEXT ĐỌC CHUỖI KẾT NỐI "MyCnn" TỪ appsettings.json
// ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
// - Sau khi chạy lệnh Scaffold DB trong Package Manager Console, mở thư mục Models/
//   xem file *Context.cs tên là gì (ví dụ: Prn232PeSu2613Context, HospitalDbContext, LibraryContext...)
// - XÓA chữ `Prn232PeSu2613Context` bên dưới và THAY BẰNG tên class Context mới của đề!
// =====================================================================
builder.Services.AddDbContext<Prn232PeSu2613Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// =====================================================================
// STEP 3: BẬT SWAGGER ĐỂ TEST API TRÊN TRÌNH DUYỆT (http://localhost:5000/swagger)
// ⚠️ [NẾU ĐỔI ĐỀ]: GIỮ NGUYÊN 100%, KHÔNG SỬA!
// =====================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
