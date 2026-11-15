using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    // [SỬA CHỖ NÀY]: Xóa các class DTO bên dưới và copy nguyên các class DTO mới dán vào
    // [XEM Ở ĐÂU]: Mở file `givenAPI/Models/DTOs.cs` hoặc cuối file `givenAPI/Controllers/*Controller.cs` trong thư mục đề cho sẵn (hoặc xem mục Schemas trên Swagger http://localhost:5100/swagger)
    public class SpecializationDTO
    {
        public int SpecID { get; set; }
        public string SpecName { get; set; } = null!;
    }

    public class TrainerSearchDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
    }

    public class BookingSessionDTO
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; } = null!;
        public int BookingID { get; set; }
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    public class TrainerDetailDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
        public List<BookingSessionDTO> BookingSessions { get; set; } = new();
    }

    // [SỬA CHỖ NÀY]: Đổi tên `TrainerController` -> `<TenController>Controller` | [XEM Ở ĐÂU]: Xem trong Index.html dòng `<form action="http://localhost:5190/...">` hoặc mục "5. Summary of Required URLs" trên đề PDF Q2
    public class TrainerController : Controller
    {
        private readonly IConfiguration _config;

        // [SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên Controller ở trên
        public TrainerController(IConfiguration config)
        {
            _config = config;
        }

        // ==================== ACTION INDEX (TRANG CHÍNH) ====================
        // [SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Trainer")]` | [XEM Ở ĐÂU]: Mục "3.1. ... Page - URL:" hoặc bảng cuối "5. Summary of Required URLs" trên đề PDF Q2
        // 2. Sửa tên 2 tham số `(int specId = 0, string? trainerName = null)` | [XEM Ở ĐÂU]: Thuộc tính `name="..."` của thẻ <select> và <input> trong file Index.html đề cho
        [HttpGet("/Trainer")]
        public async Task<IActionResult> Index(int specId = 0, string? trainerName = null)
        {
            // [GIỮ NGUYÊN] Bắt buộc đọc "GivenAPIBaseUrl" từ appsettings.json
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // [SỬA CHỖ NÀY]: Sửa đường dẫn `"/api/specializations"` + kiểu `List<SpecializationDTO>` (nếu là mảng chữ thì dùng `List<string>`)
            // [XEM Ở ĐÂU]: Mục "2. Provided APIs" trên đề PDF Q2 hoặc mở http://localhost:5100/swagger
            var specRes = await client.GetStringAsync(baseUrl + "/api/specializations");
            var specs = JsonConvert.DeserializeObject<List<SpecializationDTO>>(specRes) ?? new List<SpecializationDTO>();

            // [SỬA CHỖ NÀY]: Sửa đường dẫn API search + tên query param (`specId=`, `trainerName=`) + giữ BẮT BUỘC phép cộng chuỗi dấu `+`
            // [XEM Ở ĐÂU]: Mục "2. Provided APIs" trên đề PDF Q2 hoặc Swagger port 5100
            string searchUrl = baseUrl + "/api/trainers/search?specId=" + specId + "&trainerName=" + (trainerName ?? "");
            var trainerRes = await client.GetStringAsync(searchUrl);
            // [SỬA CHỖ NÀY]: Đổi `TrainerSearchDTO` khớp tên DTO danh sách ở trên
            var trainers = JsonConvert.DeserializeObject<List<TrainerSearchDTO>>(trainerRes) ?? new List<TrainerSearchDTO>();

            // [SỬA CHỖ NÀY]: Đổi tên biến ViewBag khớp với phần đầu file Views/<Controller>/Index.cshtml
            ViewBag.Specs = specs;
            ViewBag.SelectedSpecId = specId;
            ViewBag.TrainerName = trainerName ?? "";

            return View(trainers);
        }

        // ==================== ACTION ANALYZE (TRANG CHI TIẾT) ====================
        // [SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Trainer/Analyze/{id}")]` | [XEM Ở ĐÂU]: Link `<a href="...">` trong Index.html hoặc bảng "5. Summary of Required URLs" trên đề PDF Q2
        // 2. Sửa `"/api/trainers/" + id` và `TrainerDetailDTO` | [XEM Ở ĐÂU]: Mục "2. Provided APIs" trên đề PDF Q2 (hoặc Swagger port 5100)
        [HttpGet("/Trainer/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            var detailRes = await client.GetStringAsync(baseUrl + "/api/trainers/" + id);
            var trainer = JsonConvert.DeserializeObject<TrainerDetailDTO>(detailRes);

            return View(trainer);
        }
    }
}
