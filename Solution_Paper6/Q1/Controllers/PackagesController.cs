using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    // =========================================================================
    // STEP 1: TẠO CLASS DTO ĐỂ NHẬN DỮ LIỆU JSON KHI THÊM MỚI (CÂU 1.3 - POST)
    // Giải thích: Khi chấm câu POST, phần mềm chấm sẽ gửi 1 cục JSON (Request Body) lên.
    // Ta tạo class này chứa đúng các trường mà đề bài gửi lên trong JSON mẫu của câu POST.
    // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
    // - Xem trong đề câu 1.3 (POST) phần "Request Body" có những thuộc tính nào.
    // - XÓA 2 dòng PackageName, Price bên dưới và THAY BẰNG các thuộc tính của đề mới.
    // =========================================================================
    public class PackageCreateDto
    {
        public string? PackageName { get; set; }     // <-- [SỬA KHI ĐỔI ĐỀ]: Tên thuộc tính chuỗi trong JSON POST
        public decimal? Price { get; set; }          // <-- [SỬA KHI ĐỔI ĐỀ]: Tên thuộc tính giá/số trong JSON POST
    }

    // =========================================================================
    // STEP 2: KHAI BÁO CONTROLLER VÀ TIÊM DBCONTEXT VÀO (DEPENDENCY INJECTION)
    // =========================================================================
    [ApiController]
    public class PackagesController : ControllerBase
    {
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // Thay `Prn232PeSu2613Context` ở cả 2 dòng dưới bằng tên class *Context trong thư mục Models/
        private readonly Prn232PeSu2613Context _context;

        public PackagesController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        // =====================================================================
        // CÂU 1.1 (1.0 ĐIỂM): GET DANH SÁCH BẢNG DANH MỤC + ĐẾM SỐ LƯỢNG BẢNG CON
        // Đề hiện tại: GET /api/MembershipPackages (Lấy danh sách gói tập + đếm số Member)
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] cho giống hệt đường dẫn câu 1.1 trong đề.
        [HttpGet("api/MembershipPackages")]
        public IActionResult GetMembershipPackages()
        {
            // STEP 3.1: Truy vấn bảng danh mục từ _context và dùng .Select() tạo đối tượng trả về
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Thay `_context.MembershipPackages` bằng bảng danh mục của đề
            // - Sửa tên các thuộc tính bên trong `new { ... }` khớp với JSON mẫu trong đề bài!
            // - Chỗ `.Count`: lấy danh sách bảng con chấm `.Count` (ở đây là `p.Members.Count`)
            var result = _context.MembershipPackages
                .Select(p => new
                {
                    PackageId = p.PackageId,         // <-- [SỬA KHI ĐỔI ĐỀ]: Khóa chính
                    PackageName = p.PackageName,     // <-- [SỬA KHI ĐỔI ĐỀ]: Tên gói/danh mục
                    Price = p.Price,                 // <-- [SỬA KHI ĐỔI ĐỀ]: Giá (hoặc thuộc tính khác)
                    MemberCount = p.Members.Count    // <-- [SỬA KHI ĐỔI ĐỀ]: Đếm số lượng bảng con liên kết
                })
                .ToList();

            // STEP 3.2: Trả về mã 200 OK kèm danh sách JSON
            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.2 (1.5 ĐIỂM): GET FILTER (LỌC DỮ LIỆU + TÍNH TỔNG + BẮT LỖI 400)
        // Đề hiện tại: GET /api/filter-members?packageId=1&minDuration=50
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] đúng theo câu 1.2 của đề.
        // 2. Sửa tên 2 tham số trên hàm `FilterMembers(int? packageId, int? minDuration)`
        //    cho TRÙNG KHỚP 100% tên biến sau dấu `?` trên URL của đề bài!
        [HttpGet("api/filter-members")]
        public IActionResult FilterMembers(int? packageId, int? minDuration)
        {
            // STEP 4.1: Kiểm tra điều kiện số âm -> trả về 400 BadRequest (ăn ngay 0.25 - 0.5 điểm!)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Copy Y HỆT câu thông báo lỗi trong đề bài dán vào trong `BadRequest("...")`
            if (minDuration.HasValue && minDuration.Value < 0)
            {
                return BadRequest("minDuration cannot be negative");
            }

            // STEP 4.2: Load toàn bộ bảng chính (Members) kèm các bảng liên quan bằng .Include() và .ThenInclude()
            // MẸO ĐI THI: Dùng .ToList() kéo hết lên RAM rồi xử lý bằng vòng lặp foreach cực kỳ dễ hiểu và không sợ lỗi SQL!
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Mở file class chính trong Models/ (ở đây là Member.cs) xem bên dưới cùng có những bảng nào
            // thì .Include() hết vào đây!
            var members = _context.Members
                .Include(m => m.Package)                     // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng gói tập/danh mục (quan hệ 1-N)
                .Include(m => m.BookingDetails)              // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng chi tiết buổi tập
                    .ThenInclude(bd => bd.Booking)           // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng lịch đặt
                        .ThenInclude(b => b.Trainer)         // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng HLV
                .ToList();

            // STEP 4.3: Lọc theo tham số thứ 1 (packageId) nếu người dùng có truyền lên
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]: Thay `m.PackageId == packageId.Value` bằng cột khóa ngoại cần lọc
            if (packageId.HasValue)
            {
                members = members.Where(m => m.PackageId == packageId.Value).ToList();
            }

            // STEP 4.4: Duyệt qua từng bản ghi để tính tổng, tìm buổi tập mới nhất và lọc theo minDuration
            var result = new List<object>();

            foreach (var m in members)
            {
                // Sắp xếp các buổi tập của hội viên này theo thời gian giảm dần (buổi mới nhất đứng đầu)
                var details = m.BookingDetails
                    .OrderByDescending(bd => bd.Booking?.SessionTime)
                    .ToList();

                // Tính tổng số phút đã tập
                int totalMinutes = details.Sum(bd => bd.DurationMinutes ?? 0);

                // Nếu đề truyền vào minDuration mà tổng số phút nhỏ hơn minDuration -> Bỏ qua (continue)
                // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]: Thay điều kiện lọc tổng/số lượng nếu đề thay đổi
                if (minDuration.HasValue && totalMinutes < minDuration.Value)
                {
                    continue;
                }

                // Lấy tên HLV của buổi tập gần nhất; nếu chưa có buổi tập nào thì trả về "No Bookings"
                var lastBooking = details.FirstOrDefault()?.Booking;
                string lastTrainer = lastBooking?.Trainer?.TrainerName ?? "No Bookings";

                // Định dạng ngày kiểu DateOnly sang chuỗi "yyyy-MM-ddTHH:mm:ss" giống JSON mẫu của đề
                string joinDateStr = m.JoinDate.HasValue
                    ? m.JoinDate.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-ddTHH:mm:ss")
                    : "";

                // Thêm vào danh sách kết quả trả về (tên thuộc tính khớp 100% JSON mẫu của đề)
                result.Add(new
                {
                    MemberId = m.MemberId,
                    FullName = m.FullName,
                    JoinDate = joinDateStr,
                    PackageName = m.Package?.PackageName ?? "",
                    TotalMinutesTrained = totalMinutes,
                    LastTrainerName = lastTrainer
                });
            }

            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.3 (1.25 ĐIỂM): POST THÊM MỚI + KIỂM TRA TRÙNG TÊN
        // Đề hiện tại: POST /api/MembershipPackages
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpPost("...")] theo đúng đề.
        [HttpPost("api/MembershipPackages")]
        public IActionResult CreatePackage([FromBody] PackageCreateDto dto)
        {
            // STEP 5.1: Chuẩn hóa trường cần kiểm tra trùng (cắt khoảng trắng 2 đầu + đổi chữ thường)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Xem đề bắt kiểm tra trùng cột nào (PackageName, Email, Code...) thì thay `dto.PackageName` và `p.PackageName`
            string pkgName = (dto.PackageName ?? "").Trim();

            bool exists = _context.MembershipPackages
                .Any(p => p.PackageName != null && p.PackageName.ToLower() == pkgName.ToLower());

            if (exists)
            {
                // ⚠️ [NẾU ĐỔI ĐỀ]: Copy đúng câu báo lỗi trùng trong đề dán vào đây!
                return BadRequest("Membership package name already exists");
            }

            // STEP 5.2: Tạo đối tượng Entity mới từ DTO để lưu xuống Database
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `new MembershipPackage` bằng class Model cần thêm mới, và gán các thuộc tính từ `dto` sang.
            var pkg = new MembershipPackage
            {
                PackageName = dto.PackageName,
                Price = dto.Price
            };

            // STEP 5.3: Lưu vào DB
            _context.MembershipPackages.Add(pkg);
            _context.SaveChanges();

            // STEP 5.4: Trả về mã 201 Created kèm đối tượng vừa tạo
            return StatusCode(201, pkg);
        }

        // =====================================================================
        // CÂU 1.4 (1.25 ĐIỂM): DELETE XÓA BẢN GHI + KIỂM TRA RÀNG BUỘC KHÓA NGOẠI
        // Đề hiện tại: DELETE /api/MembershipPackages/{packageId}
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpDelete("...")] và tên tham số `int packageId` khớp với đề.
        [HttpDelete("api/MembershipPackages/{packageId}")]
        public IActionResult DeletePackage(int packageId)
        {
            // STEP 6.1: Tìm bản ghi cần xóa, nhớ .Include() bảng con để kiểm tra xem có đang được sử dụng không
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `_context.MembershipPackages`, `.Include(p => p.Members)` và `p.PackageId == packageId`
            var pkg = _context.MembershipPackages
                .Include(p => p.Members)
                .FirstOrDefault(p => p.PackageId == packageId);

            // STEP 6.2: Nếu không tìm thấy ID -> Trả về 404 NotFound (ăn 0.25 điểm)
            if (pkg == null)
            {
                return NotFound();
            }

            // STEP 6.3: Nếu gói tập đang có hội viên đăng ký (Members.Any()) -> Cấm xóa, trả về 400 BadRequest
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `pkg.Members.Any()` bằng bảng con mà đề cấm xóa khi đang có dữ liệu,
            // và copy Y HỆT câu báo lỗi trong đề vào `BadRequest("...")`
            if (pkg.Members.Any())
            {
                return BadRequest("Cannot delete membership package with active registered members");
            }

            // STEP 6.4: Xóa bản ghi và lưu thay đổi vào DB
            _context.MembershipPackages.Remove(pkg);
            _context.SaveChanges();

            // STEP 6.5: Xóa thành công -> Trả về 204 NoContent
            return NoContent();
        }
    }
}
