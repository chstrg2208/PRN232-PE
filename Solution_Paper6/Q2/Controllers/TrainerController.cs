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

    // DTO 1: Hứng dữ liệu cho Dropdown chuyên môn (/api/specializations)
    public class SpecializationDTO
    {
        public int SpecID { get; set; }
        public string SpecName { get; set; } = null!;
    }

    // DTO 2: Hứng danh sách HLV hiển thị ở trang chính (/Trainer)
    public class TrainerSearchDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
    }

    // DTO 3: Hứng từng dòng lịch dạy ở bảng dưới của trang Analyze
    public class BookingSessionDTO
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; } = null!;
        public int BookingID { get; set; }
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    // DTO 4: Hứng thông tin chi tiết 1 HLV + danh sách lịch dạy ở trang Analyze (/Trainer/Analyze/{id})
    public class TrainerDetailDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
        public List<BookingSessionDTO> BookingSessions { get; set; } = new();
    }

    // =========================================================================
    // STEP 2: TẠO CONTROLLER XỬ LÝ 2 TRANG: TRANG DANH SÁCH & TRANG CHI TIẾT (ANALYZE)
    // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
    // Đổi tên `TrainerController` theo đường dẫn mà đề yêu cầu (VD: `/Doctor` -> `DoctorController`)
    // =========================================================================
    public class TrainerController : Controller
    {
        // Biến _config dùng để đọc link "GivenAPIBaseUrl": "http://localhost:5100" từ appsettings.json
        private readonly IConfiguration _config;

        public TrainerController(IConfiguration config)
        {
            _config = config;
        }

        // =====================================================================
        // STEP 3: ACTION INDEX - HIỂN THỊ TRANG CHÍNH + LỌC DỮ LIỆU (3.5 ĐIỂM)
        // Đường dẫn hiện tại: GET /Trainer
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa `[HttpGet("/Trainer")]` thành đường dẫn trang chính của đề (VD: `[HttpGet("/Doctor")]`).
        // 2. Sửa tên 2 tham số `(int specId = 0, string? trainerName = null)` khớp với `name="..."`
        //    của thẻ select và ô input trong file giao diện `givenHTML`!
        [HttpGet("/Trainer")]
        public async Task<IActionResult> Index(int specId = 0, string? trainerName = null)
        {
            // STEP 3.1: Lấy địa chỉ http://localhost:5100 từ appsettings.json và khởi tạo HttpClient
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // STEP 3.2: Gọi API thứ 1 để lấy danh sách chuyên môn đổ vào Dropdown (thẻ <select>)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Đổi `"/api/specializations"` thành đường dẫn API lấy danh mục dropdown của đề.
            // - Đổi `SpecializationDTO` thành tên class DTO của dropdown mới.
            var specRes = await client.GetStringAsync(baseUrl + "/api/specializations");
            var specs = JsonConvert.DeserializeObject<List<SpecializationDTO>>(specRes) ?? new List<SpecializationDTO>();

            // STEP 3.3: Gọi API thứ 2 để tìm kiếm danh sách HLV hiển thị ra bảng
            // LƯU Ý QUAN TRỌNG: Đề bài yêu cầu NỐI CHUỖI TƯỜNG MINH (`+`) khi tạo URL gọi API!
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Sửa đường dẫn `"/api/trainers/search?specId=" + ...` đúng theo Swagger của givenAPI.
            string searchUrl = baseUrl + "/api/trainers/search?specId=" + specId + "&trainerName=" + (trainerName ?? "");
            var trainerRes = await client.GetStringAsync(searchUrl);
            var trainers = JsonConvert.DeserializeObject<List<TrainerSearchDTO>>(trainerRes) ?? new List<TrainerSearchDTO>();

            // STEP 3.4: Lưu danh sách dropdown và giá trị vừa chọn vào ViewBag để giữ trạng thái trên giao diện
            ViewBag.Specs = specs;
            ViewBag.SelectedSpecId = specId;
            ViewBag.TrainerName = trainerName ?? "";

            // STEP 3.5: Truyền danh sách kết quả sang View `Views/Trainer/Index.cshtml`
            return View(trainers);
        }

        // =====================================================================
        // STEP 4: ACTION ANALYZE - HIỂN THỊ TRANG PHÂN TÍCH LỊCH DẠY (1.5 ĐIỂM)
        // Đường dẫn hiện tại: GET /Trainer/Analyze/{id}
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa `[HttpGet("/Trainer/Analyze/{id}")]` theo đúng đường dẫn khi bấm nút Action trên bảng.
        // 2. Sửa `"/api/trainers/" + id` thành đường dẫn API lấy chi tiết theo ID của givenAPI.
        // 3. Sửa `TrainerDetailDTO` thành tên class DTO chi tiết của đề mới.
        [HttpGet("/Trainer/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // Gọi API lấy chi tiết 1 HLV theo id
            var detailRes = await client.GetStringAsync(baseUrl + "/api/trainers/" + id);
            var trainer = JsonConvert.DeserializeObject<TrainerDetailDTO>(detailRes);

            // Truyền đối tượng sang View `Views/Trainer/Analyze.cshtml`
            return View(trainer);
        }
    }
}
