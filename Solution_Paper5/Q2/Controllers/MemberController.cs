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
    // XÓA 3 class MemberSearchDTO, MemberSessionDTO, MemberDetailDTO bên dưới
    // và DÁN các class DTO tương ứng của đề mới vào!
    // =========================================================================

    // DTO 1: Hứng danh sách hiển thị ở trang chính (/Member)
    public class MemberSearchDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
    }

    // DTO 2: Hứng từng dòng lịch sử buổi tập ở bảng dưới của trang Analyze
    public class MemberSessionDTO
    {
        public int BookingID { get; set; }
        public string TrainerName { get; set; } = null!;
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    // DTO 3: Hứng thông tin chi tiết 1 hội viên + danh sách buổi tập ở trang Analyze (/Member/Analyze/{id})
    public class MemberDetailDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
        public decimal PackagePrice { get; set; }
        public List<MemberSessionDTO> Sessions { get; set; } = new();
    }

    // =========================================================================
    // STEP 2: TẠO CONTROLLER XỬ LÝ 2 TRANG: TRANG DANH SÁCH & TRANG CHI TIẾT (ANALYZE)
    // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
    // Đổi tên `MemberController` theo đường dẫn mà đề yêu cầu (VD: `/Student` -> `StudentController`)
    // =========================================================================
    public class MemberController : Controller
    {
        // Biến _config dùng để đọc link "GivenAPIBaseUrl": "http://localhost:5100" từ appsettings.json
        private readonly IConfiguration _config;

        public MemberController(IConfiguration config)
        {
            _config = config;
        }

        // =====================================================================
        // STEP 3: ACTION INDEX - HIỂN THỊ TRANG CHÍNH + LỌC DỮ LIỆU (3.5 ĐIỂM)
        // Đường dẫn hiện tại: GET /Member
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa `[HttpGet("/Member")]` thành đường dẫn trang chính của đề (VD: `[HttpGet("/Student")]`).
        // 2. Sửa tên 2 tham số `(string? memberName, string? bookingStatus)` khớp với `name="..."`
        //    của ô input và thẻ select trong file giao diện `givenHTML`!
        [HttpGet("/Member")]
        public async Task<IActionResult> Index(string? memberName, string? bookingStatus)
        {
            // STEP 3.1: Lấy địa chỉ http://localhost:5100 từ appsettings.json và khởi tạo HttpClient
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // STEP 3.2: Gọi API thứ 1 để lấy dữ liệu đổ vào Dropdown (thẻ <select>)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Đổi `"/api/booking-statuses"` thành đường dẫn API lấy danh mục dropdown của đề.
            // - Nếu API trả về mảng chuỗi `["Đã tập", ...]` thì giữ `List<string>`.
            // - Nếu API trả về mảng object `[{"specID":1, "specName":"Yoga"}]` thì đổi `List<string>` thành `List<TenClassDTO>`.
            var statusRes = await client.GetStringAsync(baseUrl + "/api/booking-statuses");
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusRes) ?? new List<string>();

            // STEP 3.3: Gọi API thứ 2 để tìm kiếm danh sách hiển thị ra bảng
            // LƯU Ý QUAN TRỌNG: Đề bài yêu cầu NỐI CHUỖI TƯỜNG MINH (`+`) khi tạo URL gọi API!
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Xem giá trị mặc định khi chưa chọn dropdown là gì (ở đây là `"All statuses"`).
            // - Sửa đường dẫn `"/api/members/search?memberName=" + ...` đúng theo Swagger của givenAPI.
            string selectedStatus = string.IsNullOrEmpty(bookingStatus) ? "All statuses" : bookingStatus;
            string searchUrl = baseUrl + "/api/members/search?memberName=" + (memberName ?? "") + "&bookingStatus=" + selectedStatus;

            var memberRes = await client.GetStringAsync(searchUrl);
            var members = JsonConvert.DeserializeObject<List<MemberSearchDTO>>(memberRes) ?? new List<MemberSearchDTO>();

            // STEP 3.4: Lưu danh sách dropdown và giá trị vừa chọn vào ViewBag để giữ trạng thái trên giao diện
            ViewBag.Statuses = statuses;
            ViewBag.SelectedStatus = selectedStatus;
            ViewBag.MemberName = memberName ?? "";

            // STEP 3.5: Truyền danh sách kết quả sang View `Views/Member/Index.cshtml`
            return View(members);
        }

        // =====================================================================
        // STEP 4: ACTION ANALYZE - HIỂN THỊ TRANG PHÂN TÍCH CHI TIẾT (1.5 ĐIỂM)
        // Đường dẫn hiện tại: GET /Member/Analyze/{id}
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa `[HttpGet("/Member/Analyze/{id}")]` theo đúng đường dẫn khi bấm nút Action trên bảng.
        // 2. Sửa `"/api/members/" + id` thành đường dẫn API lấy chi tiết theo ID của givenAPI.
        // 3. Sửa `MemberDetailDTO` thành tên class DTO chi tiết của đề mới.
        [HttpGet("/Member/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // Gọi API lấy chi tiết 1 bản ghi theo id
            var detailRes = await client.GetStringAsync(baseUrl + "/api/members/" + id);
            var member = JsonConvert.DeserializeObject<MemberDetailDTO>(detailRes);

            // Truyền đối tượng sang View `Views/Member/Analyze.cshtml`
            return View(member);
        }
    }
}
