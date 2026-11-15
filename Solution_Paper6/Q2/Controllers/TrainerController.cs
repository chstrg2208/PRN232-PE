using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    // =========================================================================
    // STEP 1: TẠO CÁC CLASS DTO ĐỂ HỨNG DỮ LIỆU JSON TỪ givenAPI TRẢ VỀ
    // MẸO CỰC NHANH KHI ĐI THI:
    // Mở thư mục đề cho sẵn `GivenAPIs/Controllers/GymApiController.cs` (hoặc chạy thử
    // givenAPI trên trình duyệt `http://localhost:5100/swagger`), copy nguyên các class DTO
    // ở cuối file của họ dán vào đây! Không cần tự gõ!
    // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
    // XÓA 4 class SpecializationDTO, TrainerSearchDTO, BookingSessionDTO, TrainerDetailDTO bên dưới
    // và DÁN các class DTO tương ứng của đề mới vào!
    // =========================================================================

    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Toàn bộ các class DTO bên dưới: copy từ `givenAPI/Models/DTOs.cs` hoặc `givenAPI/Controllers/*Controller.cs` của đề mới dán sang!
    // DTO 1: Hứng dữ liệu cho Dropdown chuyên môn (/api/specializations)
    public class SpecializationDTO
    {
        public int SpecID { get; set; }                  // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID danh mục trong JSON Swagger
        public string SpecName { get; set; } = null!;    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên danh mục trong JSON Swagger
    }

    // DTO 2: Hứng danh sách hiển thị ở bảng trang chính (/Trainer)
    public class TrainerSearchDTO
    {
        public int TrainerID { get; set; }               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Khóa chính của dòng trên bảng (dùng cho id="td_..._@t.TrainerID")
        public string TrainerName { get; set; } = null!; // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột tên hiển thị
        public string Email { get; set; } = null!;       // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột phụ 1 (đổi/thêm/bớt theo bảng HTML mẫu)
        public int ExperienceYears { get; set; }         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột phụ 2
        public List<SpecializationDTO> Specializations { get; set; } = new(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Danh sách tag con (nếu bảng có cột nhiều tag)
    }

    // DTO 3: Hứng từng dòng lịch sử ở bảng dưới của trang Analyze
    public class BookingSessionDTO
    {
        public int MemberID { get; set; }                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID phục vụ ghép id HTML (vd: td_member_@s.MemberID)
        public string MemberName { get; set; } = null!;  // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên đối tượng trong bảng chi tiết
        public int BookingID { get; set; }               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID giao dịch phục vụ ghép id HTML (vd: td_time_@s.BookingID)
        public DateTime SessionTime { get; set; }        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thời gian giao dịch
        public int DurationMinutes { get; set; }         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Chỉ số tính toán / phân loại (High vs Standard)
        public string Status { get; set; } = null!;      // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Trạng thái (nếu DTO của givenAPI có)
    }

    // DTO 4: Hứng thông tin chi tiết 1 đối tượng + danh sách con ở trang Analyze (/Trainer/Analyze/{id})
    public class TrainerDetailDTO
    {
        public int TrainerID { get; set; }               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID thẻ Card phía trên
        public string TrainerName { get; set; } = null!; // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên thẻ Card phía trên
        public int ExperienceYears { get; set; }         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thông số thẻ Card phía trên
        public List<SpecializationDTO> Specializations { get; set; } = new(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Danh sách con 1 (nếu có)
        public List<BookingSessionDTO> BookingSessions { get; set; } = new(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Danh sách bảng chi tiết bên dưới (phải trùng tên property JSON!)
    }

    // =========================================================================
    // STEP 2: TẠO CONTROLLER XỬ LÝ 2 TRANG: TRANG DANH SÁCH & TRANG CHI TIẾT (ANALYZE)
    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên `TrainerController` -> `<TenDoiTuong>Controller` (vd: `/Doctor` -> `DoctorController` + đổi tên thư mục Views/Doctor!)
    // =========================================================================
    public class TrainerController : Controller
    {
        // Biến _config dùng để đọc link "GivenAPIBaseUrl": "http://localhost:5100" từ appsettings.json
        private readonly IConfiguration _config;

        public TrainerController(IConfiguration config) // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên class Controller
        {
            _config = config;
        }

        // =====================================================================
        // STEP 3: ACTION INDEX - HIỂN THỊ TRANG CHÍNH + LỌC DỮ LIỆU (3.5 ĐIỂM)
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. `[HttpGet("/Trainer")]` -> sửa đúng route trang chính trong đề PDF (vd: `[HttpGet("/Doctor")]`)
        // 2. Tên 2 tham số `(int specId = 0, string? trainerName = null)` PHẢI KHỚP 100% thuộc tính `name="..."` của <select> & <input> trong HTML!
        [HttpGet("/Trainer")]
        public async Task<IActionResult> Index(int specId = 0, string? trainerName = null)
        {
            // STEP 3.1: Lấy địa chỉ API từ appsettings.json (key bắt buộc trong barem: "GivenAPIBaseUrl")
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // STEP 3.2: Gọi API thứ 1 để lấy danh sách đổ vào Dropdown (thẻ <select>)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa `"/api/specializations"` theo Swagger givenAPI; nếu API trả mảng string thì dùng `List<string>` thay cho `List<SpecializationDTO>`
            var specRes = await client.GetStringAsync(baseUrl + "/api/specializations");
            var specs = JsonConvert.DeserializeObject<List<SpecializationDTO>>(specRes) ?? new List<SpecializationDTO>();

            // STEP 3.3: Gọi API thứ 2 để tìm kiếm danh sách hiển thị ra bảng
            // LƯU Ý BẮT BUỘC: Phải dùng nối chuỗi `+` tường minh (KHÔNG dùng $"...") theo file Lưu ý PE!
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa đường dẫn & tên query params (`specId=`, `trainerName=`) khớp 100% Swagger của givenAPI!
            string searchUrl = baseUrl + "/api/trainers/search?specId=" + specId + "&trainerName=" + (trainerName ?? "");
            var trainerRes = await client.GetStringAsync(searchUrl);
            var trainers = JsonConvert.DeserializeObject<List<TrainerSearchDTO>>(trainerRes) ?? new List<TrainerSearchDTO>(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên DTO danh sách

            // STEP 3.4: Lưu danh sách dropdown và giá trị vừa chọn vào ViewBag để giữ trạng thái trên giao diện
            ViewBag.Specs = specs;                           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên ViewBag nhớ khớp bên View Index.cshtml
            ViewBag.SelectedSpecId = specId;                 // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Giá trị đã chọn của <select>
            ViewBag.TrainerName = trainerName ?? "";         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Giá trị đã nhập của <input>

            // STEP 3.5: Truyền danh sách kết quả sang View `Views/Trainer/Index.cshtml`
            return View(trainers);
        }

        // =====================================================================
        // STEP 4: ACTION ANALYZE - HIỂN THỊ TRANG PHÂN TÍCH CHI TIẾT (1.5 ĐIỂM)
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Trainer/Analyze/{id}")]` khớp link `<a href="/.../Analyze/@...">` trong HTML.
        // 2. Sửa `"/api/trainers/" + id` đúng endpoint chi tiết trong Swagger givenAPI (giữ dấu `+`).
        // 3. Sửa `TrainerDetailDTO` thành DTO chi tiết mới.
        [HttpGet("/Trainer/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // Gọi API lấy chi tiết 1 bản ghi theo id
            var detailRes = await client.GetStringAsync(baseUrl + "/api/trainers/" + id); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi path `"/api/trainers/"`
            var trainer = JsonConvert.DeserializeObject<TrainerDetailDTO>(detailRes);       // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi class `TrainerDetailDTO`

            // Truyền đối tượng sang View `Views/Trainer/Analyze.cshtml`
            return View(trainer);
        }
    }
}
