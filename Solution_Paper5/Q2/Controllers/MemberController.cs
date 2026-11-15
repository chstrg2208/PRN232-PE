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

    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Toàn bộ các class DTO bên dưới: copy từ `givenAPI/Models/DTOs.cs` hoặc `givenAPI/Controllers/*Controller.cs` của đề mới dán sang!
    // DTO 1: Hứng danh sách hiển thị ở trang chính (/Member)
    public class MemberSearchDTO
    {
        public int MemberID { get; set; }                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Khóa chính của dòng (dùng gắn vào `id="td_..._@m.MemberID"`)
        public string FullName { get; set; } = null!;    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên hiển thị trên bảng
        public DateTime JoinDate { get; set; }           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột ngày tháng trên bảng
        public string PackageName { get; set; } = null!; // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên gói / danh mục trên bảng
    }

    // DTO 2: Hứng từng dòng lịch sử ở bảng dưới của trang Analyze
    public class MemberSessionDTO
    {
        public int BookingID { get; set; }               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID giao dịch gắn vào `id="td_..._@s.BookingID"`
        public string TrainerName { get; set; } = null!; // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên đối tượng liên kết trong bảng dưới
        public DateTime SessionTime { get; set; }        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thời gian giao dịch
        public int DurationMinutes { get; set; }         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Chỉ số tính tiền / tính mức phân loại
        public string Status { get; set; } = null!;      // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Trạng thái (nếu DTO có)
    }

    // DTO 3: Hứng thông tin chi tiết 1 đối tượng + danh sách con ở trang Analyze (/Member/Analyze/{id})
    public class MemberDetailDTO
    {
        public int MemberID { get; set; }                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: ID trên Card phía trên
        public string FullName { get; set; } = null!;    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên trên Card phía trên
        public DateTime JoinDate { get; set; }           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Ngày trên Card phía trên
        public string PackageName { get; set; } = null!; // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên gói trên Card phía trên
        public decimal PackagePrice { get; set; }        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Giá trị / số tiền trên Card
        public List<MemberSessionDTO> Sessions { get; set; } = new(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Danh sách chi tiết bảng dưới (tên property phải trùng JSON!)
    }

    // =========================================================================
    // STEP 2: TẠO CONTROLLER XỬ LÝ 2 TRANG: TRANG DANH SÁCH & TRANG CHI TIẾT (ANALYZE)
    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên `MemberController` -> `<TenDoiTuong>Controller` (VD: `/Student` -> `StudentController` + đổi tên thư mục `Views/Student`)
    // =========================================================================
    public class MemberController : Controller
    {
        // Biến _config dùng để đọc link "GivenAPIBaseUrl": "http://localhost:5100" từ appsettings.json
        private readonly IConfiguration _config;

        public MemberController(IConfiguration config) // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên Controller
        {
            _config = config;
        }

        // =====================================================================
        // STEP 3: ACTION INDEX - HIỂN THỊ TRANG CHÍNH + LỌC DỮ LIỆU (3.5 ĐIỂM)
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Member")]` thành đường dẫn trang chính của đề (VD: `[HttpGet("/Student")]`).
        // 2. Sửa tên 2 tham số `(string? memberName, string? bookingStatus)` KHỚP 100% với `name="..."` của <input> & <select> trong `givenHTML`!
        [HttpGet("/Member")]
        public async Task<IActionResult> Index(string? memberName, string? bookingStatus)
        {
            // STEP 3.1: Lấy địa chỉ API từ appsettings.json (key bắt buộc: "GivenAPIBaseUrl")
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // STEP 3.2: Gọi API thứ 1 để lấy dữ liệu đổ vào Dropdown (thẻ <select>)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
            // - Đổi `"/api/booking-statuses"` theo Swagger của givenAPI.
            // - Nếu API trả mảng chuỗi `["A", "B"]` giữ `List<string>`; nếu trả object `[{id, name}]` đổi thành `List<TenClassDTO>`.
            var statusRes = await client.GetStringAsync(baseUrl + "/api/booking-statuses");
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusRes) ?? new List<string>();

            // STEP 3.3: Gọi API thứ 2 để tìm kiếm danh sách hiển thị ra bảng
            // LƯU Ý BẮT BUỘC: Dùng NỐI CHUỖI TƯỜNG MINH (`+`), KHÔNG dùng `$"..."`!
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
            // - Xem option "Tất cả" trên HTML / Swagger mặc định là chữ gì (`"All statuses"` hay `"All"` hay `0`).
            // - Đổi đường dẫn `"/api/members/search?memberName=" + ...` khớp 100% Swagger givenAPI!
            string selectedStatus = string.IsNullOrEmpty(bookingStatus) ? "All statuses" : bookingStatus;
            string searchUrl = baseUrl + "/api/members/search?memberName=" + (memberName ?? "") + "&bookingStatus=" + selectedStatus;

            var memberRes = await client.GetStringAsync(searchUrl);
            var members = JsonConvert.DeserializeObject<List<MemberSearchDTO>>(memberRes) ?? new List<MemberSearchDTO>(); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên DTO danh sách

            // STEP 3.4: Lưu danh sách dropdown và giá trị vừa chọn vào ViewBag để giữ trạng thái trên giao diện
            ViewBag.Statuses = statuses;                     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên ViewBag nhớ khớp bên View Index.cshtml
            ViewBag.SelectedStatus = selectedStatus;         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Giá trị đã chọn của <select>
            ViewBag.MemberName = memberName ?? "";           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Giá trị đã nhập của <input>

            // STEP 3.5: Truyền danh sách kết quả sang View `Views/Member/Index.cshtml`
            return View(members);
        }

        // =====================================================================
        // STEP 4: ACTION ANALYZE - HIỂN THỊ TRANG PHÂN TÍCH CHI TIẾT (1.5 ĐIỂM)
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. Sửa `[HttpGet("/Member/Analyze/{id}")]` theo link `<a href="/.../Analyze/@...">` trong HTML.
        // 2. Sửa `"/api/members/" + id` thành endpoint lấy chi tiết theo ID trong Swagger givenAPI (giữ dấu `+`).
        // 3. Sửa `MemberDetailDTO` thành class DTO chi tiết mới.
        [HttpGet("/Member/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // Gọi API lấy chi tiết 1 bản ghi theo id
            var detailRes = await client.GetStringAsync(baseUrl + "/api/members/" + id); // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi path `"/api/members/"`
            var member = JsonConvert.DeserializeObject<MemberDetailDTO>(detailRes);       // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `MemberDetailDTO`

            // Truyền đối tượng sang View `Views/Member/Analyze.cshtml`
            return View(member);
        }
    }
}
