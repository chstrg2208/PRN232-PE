var builder = WebApplication.CreateBuilder(args);

// [GIỮ NGUYÊN] Không sửa đoạn bên dưới
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// [SỬA]: Đổi chữ `Member` thành tên Controller | [XEM Ở ĐÂU]: Mở file Index.html đề cho, xem thẻ <form action="http://localhost:5190/..."> hoặc mục URL trên đề PDF Q2
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Member}/{action=Index}/{id?}");

app.Run();
