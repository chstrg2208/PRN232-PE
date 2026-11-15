using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    // [SỬA CHỖ NÀY]: Xóa các class DTO bên dưới và copy nguyên các class DTO mới dán vào
    // [XEM Ở ĐÂU]: Mở file `givenAPI/Models/DTOs.cs` hoặc cuối file `givenAPI/Controllers/*Controller.cs` trong thư mục đề cho sẵn (hoặc xem mục Schemas trên Swagger http://localhost:5100/swagger)
    public class MemberSearchDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
    }

    public class MemberSessionDTO
    {
        public int BookingID { get; set; }
        public string TrainerName { get; set; } = null!;
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    public class MemberDetailDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
        public decimal PackagePrice { get; set; }
        public List<MemberSessionDTO> Sessions { get; set; } = new();
    }

    // [SỬA CHỖ NÀY]: Đổi tên `MemberController` -> `<TenController>Controller` | [XEM Ở ĐÂU]: Xem trong Index.html dòng `<form action="http://localhost:5190/...">` hoặc mục "5. Summary of Required URLs" trên đề PDF Q2
    public class MemberController : Controller
    {
        private readonly IConfiguration _config;

        // [SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên Controller ở trên
        public MemberController(IConfiguration config)
        {
            _config = config;
        }

        // ==================== ACTION INDEX (TRANG CHÍNH) ====================
        // [SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Member")]` | [XEM Ở ĐÂU]: Mục "3.1. ... Page - URL:" hoặc bảng cuối "5. Summary of Required URLs" trên đề PDF Q2
        // 2. Sửa tên 2 tham số `(string? memberName, string? bookingStatus)` | [XEM Ở ĐÂU]: Thuộc tính `name="..."` của thẻ <input> và <select> trong file Index.html đề cho
        [HttpGet("/Member")]
        public async Task<IActionResult> Index(string? memberName, string? bookingStatus)
        {
            // [GIỮ NGUYÊN] Bắt buộc đọc "GivenAPIBaseUrl" từ appsettings.json
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // [SỬA CHỖ NÀY]: Sửa đường dẫn `"/api/booking-statuses"` + kiểu `List<string>` (nếu là mảng object thì đổi thành `List<TenDTO>`)
            // [XEM Ở ĐÂU]: Mục "2. Provided APIs" trên đề PDF Q2 hoặc mở http://localhost:5100/swagger
            var statusRes = await client.GetStringAsync(baseUrl + "/api/booking-statuses");
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusRes) ?? new List<string>();

            // [SỬA CHỖ NÀY]: Sửa giá trị mặc định `"All statuses"` + đường dẫn API search + giữ BẮT BUỘC phép cộng chuỗi dấu `+`
            // [XEM Ở ĐÂU]: Thẻ <option> đầu tiên trong file Index.html đề cho + mục "2. Provided APIs" trên đề PDF Q2 (hoặc Swagger port 5100)
            string selectedStatus = string.IsNullOrEmpty(bookingStatus) ? "All statuses" : bookingStatus;
            string searchUrl = baseUrl + "/api/members/search?memberName=" + (memberName ?? "") + "&bookingStatus=" + selectedStatus;

            var memberRes = await client.GetStringAsync(searchUrl);
            // [SỬA CHỖ NÀY]: Đổi `MemberSearchDTO` khớp tên DTO danh sách ở trên
            var members = JsonConvert.DeserializeObject<List<MemberSearchDTO>>(memberRes) ?? new List<MemberSearchDTO>();

            // [SỬA CHỖ NÀY]: Đổi tên biến ViewBag khớp với phần đầu file Views/<Controller>/Index.cshtml
            ViewBag.Statuses = statuses;
            ViewBag.SelectedStatus = selectedStatus;
            ViewBag.MemberName = memberName ?? "";

            return View(members);
        }

        // ==================== ACTION ANALYZE (TRANG CHI TIẾT) ====================
        // [SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Member/Analyze/{id}")]` | [XEM Ở ĐÂU]: Link `<a href="...">` trong Index.html hoặc bảng "5. Summary of Required URLs" trên đề PDF Q2
        // 2. Sửa `"/api/members/" + id` và `MemberDetailDTO` | [XEM Ở ĐÂU]: Mục "2. Provided APIs" trên đề PDF Q2 (hoặc Swagger port 5100)
        [HttpGet("/Member/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            var detailRes = await client.GetStringAsync(baseUrl + "/api/members/" + id);
            var member = JsonConvert.DeserializeObject<MemberDetailDTO>(detailRes);

            return View(member);
        }
    }
}
