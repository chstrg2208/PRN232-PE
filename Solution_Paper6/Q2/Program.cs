var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// STEP 1: BẬT TÍNH NĂNG MVC (CONTROLLER + RAZOR VIEW)
// Giải thích: Template gốc của nhà trường chỉ có AddControllers() (không chạy được View .cshtml).
// Ta đổi thành AddControllersWithViews() để chạy được giao diện web.
// ⚠️ [NẾU ĐỔI ĐỀ]: GIỮ NGUYÊN 100%, KHÔNG CẦN SỬA!
// =====================================================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// STEP 2: Cho phép đọc file tĩnh (CSS/JS nếu có) và bật định tuyến URL
app.UseStaticFiles();
app.UseRouting();

// =====================================================================
// STEP 3: ĐỊNH TUYẾN MẶC ĐỊNH CHO WEB MVC
// ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
// Xem đề yêu cầu vào đường dẫn nào (ví dụ: `/Trainer`, `/Member`, `/Doctor`, `/Course`...)
// thì thay chữ `Trainer` ở `controller=Trainer` bên dưới bằng tên Controller đó!
// =====================================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Trainer}/{action=Index}/{id?}");

app.Run();
