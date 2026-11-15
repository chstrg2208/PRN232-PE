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
    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: DTO hứng dữ liệu JSON khi POST tạo mới
    // -> Mở đề PDF phần 1.3 (Create) xem Request Body JSON có những trường nào thì khai báo đúng tên + kiểu dữ liệu ở đây
    public class PackageCreateDto
    {
        public string? PackageName { get; set; }     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên theo field chữ trong JSON POST (vd: CourseName, DepartmentName...)
        public decimal? Price { get; set; }          // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên/kiểu theo field số/giá trong JSON POST (vd: Fee, ExperienceYears...; bỏ nếu đề không gửi)
    }

    // =========================================================================
    // STEP 2: KHAI BÁO CONTROLLER VÀ TIÊM DBCONTEXT VÀO (DEPENDENCY INJECTION)
    // =========================================================================
    [ApiController]
    public class PackagesController : ControllerBase
    {
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // Thay `Prn232PeSu2613Context` ở cả 2 dòng dưới bằng tên class *Context trong thư mục Models/
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên class DbContext (xem trong thư mục Models/ hoặc file Program.cs sau khi chạy dotnet ef dbcontext scaffold)
        private readonly Prn232PeSu2613Context _context;

        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên class Controller + đúng kiểu DbContext
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
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Route câu 1.1 - copy đúng path trong đề PDF (vd: "api/Specializations", "api/Categories")
        [HttpGet("api/MembershipPackages")]
        public IActionResult GetMembershipPackages()
        {
            // STEP 3.1: Truy vấn bảng danh mục từ _context và dùng .Select() tạo đối tượng trả về
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên DbSet (_context.TenBangDanhMuc) + tên các key JSON trả về đúng theo JSON mẫu câu 1.1
            var result = _context.MembershipPackages
                .Select(p => new
                {
                    PackageId = p.PackageId,         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên key JSON + cột Khóa chính (vd: SpecId = s.SpecId)
                    PackageName = p.PackageName,     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên key JSON + cột Tên danh mục (vd: CategoryName = c.CategoryName)
                    Price = p.Price,                 // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột phụ (giá/mô tả...; nếu JSON mẫu không có thì xóa, có thêm cột thì thêm vào)
                    MemberCount = p.Members.Count    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên key đếm số lượng + Navigation Collection nối sang bảng con (p.TenBangCon.Count)
                })
                .ToList();

            // STEP 3.2: Trả về mã 200 OK kèm danh sách JSON
            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.2 (1.5 ĐIỂM): GET FILTER (LỌC DỮ LIỆU + TÍNH TỔNG + SẮP XẾP + BẮT LỖI 400)
        // Đề hiện tại: GET /api/filter-members?packageId=1&minDuration=50
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] đúng theo câu 1.2 trong đề PDF (vd: "api/filter-students").
        // 2. Sửa tên 2 tham số hàm `(int? packageId, int? minDuration)` KHỚP 100% tên biến query sau dấu `?` trong URL đề bài!
        [HttpGet("api/filter-members")]
        public IActionResult FilterMembers(int? packageId, int? minDuration)
        {
            // STEP 4.1: Kiểm tra tham số âm -> trả về 400 BadRequest (ăn ngay 0.25 - 0.5 điểm!)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên biến cần check âm (vd: minExperience, minPrice...) và copy Y HỆT chuỗi báo lỗi trong đề PDF!
            if (minDuration.HasValue && minDuration.Value < 0)
            {
                return BadRequest("minDuration cannot be negative");
            }

            // STEP 4.2: Load toàn bộ bảng chính (Members) kèm các bảng liên quan bằng .Include()/.ThenInclude() và SẮP XẾP
            // MẸO ĐI THI: Dùng .ToList() kéo hết lên RAM rồi xử lý bằng vòng lặp foreach cực kỳ dễ sửa và không lo lỗi dịch SQL của EF Core!
            var members = _context.Members                   // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi bảng chính (vd: _context.Trainers, _context.Orders...)
                .Include(m => m.Package)                     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Quan hệ 1-N sang bảng danh mục (xóa/đổi nếu tên khác)
                .Include(m => m.BookingDetails)              // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng chi tiết giao dịch (mở file Model chính trong Models/ xem tên ICollection)
                    .ThenInclude(bd => bd.Booking)           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng cha của chi tiết (vd: Booking, Order, Appointment...)
                        .ThenInclude(b => b.Trainer)         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng liên quan cấp 3 cần lấy tên (vd: Trainer, Doctor...)
                .OrderByDescending(m => m.JoinDate)          // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đề PDF câu 1.2 yêu cầu sort theo cột nào tăng (.OrderBy) hay giảm (.OrderByDescending) thì đổi ở đây!
                .ToList();

            // STEP 4.3: Lọc theo tham số thứ 1 (packageId) nếu người dùng có truyền lên
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Nếu là 1-N dùng `m.ForeignKeyId == param.Value`; nếu là N-N dùng `m.Collection.Any(x => x.Id == param.Value)`
            if (packageId.HasValue)
            {
                members = members.Where(m => m.PackageId == packageId.Value).ToList();
            }

            // STEP 4.4: Duyệt qua từng bản ghi để tính tổng, tìm buổi tập mới nhất và lọc theo minDuration
            var result = new List<object>();

            foreach (var m in members)
            {
                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thay `m.BookingDetails` & `bd.Booking?.SessionTime` bằng bảng chi tiết và cột ngày giờ cần tìm bản ghi mới nhất
                var details = m.BookingDetails
                    .OrderByDescending(bd => bd.Booking?.SessionTime)
                    .ToList();

                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tính tổng cột đề yêu cầu (vd: Sum DurationMinutes, Amount, Quantity...); nhớ `?? 0` để tránh null
                int totalMinutes = details.Sum(bd => bd.DurationMinutes ?? 0);

                // Nếu đề truyền vào minDuration mà tổng số phút nhỏ hơn minDuration -> Bỏ qua (continue)
                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thay tên tham số & tên biến tổng nếu đề lọc theo số buổi / tổng tiền / số năm...
                if (minDuration.HasValue && totalMinutes < minDuration.Value)
                {
                    continue;
                }

                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Lấy tên đối tượng gần nhất; check kỹ đề PDF xem khi rỗng trả về "No Trainers" hay "No Bookings" hay gì!
                var lastBooking = details.FirstOrDefault()?.Booking;
                string lastTrainer = lastBooking?.Trainer?.TrainerName ?? "No Trainers";

                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Nếu cột ngày trong Model scaffold là `DateOnly?` thì dùng `.ToDateTime(TimeOnly.MinValue).ToString(...)`;
                // còn nếu là `DateTime?` thì đổi thành `m.JoinDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")`
                string joinDateStr = m.JoinDate.HasValue
                    ? m.JoinDate.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-ddTHH:mm:ss")
                    : "";

                // Thêm vào danh sách kết quả trả về (tên thuộc tính PHẢI KHỚP 100% JSON mẫu trong đề PDF)
                result.Add(new
                {
                    MemberId = m.MemberId,                   // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + khóa chính bảng chính
                    FullName = m.FullName,                   // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + cột tên bảng chính
                    JoinDate = joinDateStr,                  // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + chuỗi ngày đã format (hoặc đổi cột ngày khác)
                    PackageName = m.Package?.PackageName ?? "", // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + tên danh mục cha
                    TotalMinutesTrained = totalMinutes,      // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + biến tổng vừa tính
                    LastTrainerName = lastTrainer            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + biến tên liên kết gần nhất
                });
            }

            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.3 (1.25 ĐIỂM): POST THÊM MỚI + KIỂM TRA TRÙNG TÊN
        // Đề hiện tại: POST /api/MembershipPackages
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa đường dẫn trong [HttpPost("...")] đúng theo câu 1.3 của đề PDF
        [HttpPost("api/MembershipPackages")]
        public IActionResult CreatePackage([FromBody] PackageCreateDto dto)
        {
            // STEP 5.1: Chuẩn hóa trường cần kiểm tra trùng (cắt khoảng trắng 2 đầu + đổi chữ thường)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `dto.PackageName` thành field cần check trùng (vd: dto.Email, dto.CategoryName, dto.Code...)
            string pkgName = (dto.PackageName ?? "").Trim();

            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages` và `p.PackageName` đúng bảng + cột check trùng
            bool exists = _context.MembershipPackages
                .Any(p => p.PackageName != null && p.PackageName.ToLower() == pkgName.ToLower());

            if (exists)
            {
                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Copy Y HỆT câu báo lỗi trùng trong đề PDF dán vào đây!
                return BadRequest("Membership package name already exists");
            }

            // STEP 5.2: Tạo đối tượng Entity mới từ DTO để lưu xuống Database
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `new MembershipPackage` thành tên class Entity trong Models/ và map đủ các cột POST
            var pkg = new MembershipPackage
            {
                PackageName = dto.PackageName,               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Map thuộc tính 1 từ dto sang Model
                Price = dto.Price                            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Map thuộc tính 2 từ dto sang Model (thêm/bớt tùy DTO)
            };

            // STEP 5.3: Lưu vào DB
            _context.MembershipPackages.Add(pkg);            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages` theo tên bảng
            _context.SaveChanges();

            // STEP 5.4: Trả về mã 201 Created kèm đối tượng vừa tạo
            return StatusCode(201, pkg);
        }

        // =====================================================================
        // CÂU 1.4 (1.25 ĐIỂM): DELETE XÓA BẢN GHI + KIỂM TRA RÀNG BUỘC KHÓA NGOẠI
        // Đề hiện tại: DELETE /api/MembershipPackages/{packageId}
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa route [HttpDelete("...")] và tên biến `{packageId}` khớp tên tham số hàm `int packageId`
        [HttpDelete("api/MembershipPackages/{packageId}")]
        public IActionResult DeletePackage(int packageId)
        {
            // STEP 6.1: Tìm bản ghi cần xóa, nhớ .Include() bảng con để kiểm tra xem có đang được sử dụng không
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages`, `.Include(p => p.Members)` và điều kiện khóa chính `p.PackageId == packageId`
            var pkg = _context.MembershipPackages
                .Include(p => p.Members)
                .FirstOrDefault(p => p.PackageId == packageId);

            // STEP 6.2: Nếu không tìm thấy ID -> Trả về 404 NotFound (ăn 0.25 điểm)
            if (pkg == null)
            {
                return NotFound();
            }

            // STEP 6.3: Nếu đang có dữ liệu con liên kết -> Cấm xóa, trả về 400 BadRequest
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `pkg.Members.Any()` thành bảng con cấm xóa khi còn dữ liệu, và copy Y HỆT câu báo lỗi trong đề PDF!
            if (pkg.Members.Any())
            {
                return BadRequest("Cannot delete membership package with active registered members");
            }

            // STEP 6.4: Xóa bản ghi và lưu thay đổi vào DB
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Nếu bảng bị xóa có quan hệ N-N (giống Trainer - Specs), thêm `pkg.BangTrungGian.Clear();` trước khi `.Remove(pkg)`
            _context.MembershipPackages.Remove(pkg);
            _context.SaveChanges();

            // STEP 6.5: Xóa thành công -> Trả về 204 NoContent
            return NoContent();
        }
    }
}
