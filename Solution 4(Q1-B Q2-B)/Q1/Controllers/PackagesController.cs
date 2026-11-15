using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    // [SỬA CHỖ NÀY]: Xóa/thêm/đổi các dòng bên trong class cho khớp JSON gửi lên
    // [XEM Ở ĐÂU]: Đề PDF Q1 mục "3. POST /api/..." -> nhìn hình ô "Request body" màu xanh lá & dòng "Accept ... inside the request body"; kiểu dữ liệu copy từ Q1/Models/*.cs
    public class PackageCreateDto
    {
        public string? PackageName { get; set; }     // [SỬA]: Tên & kiểu | [XEM Ở ĐÂU]: ô "Request body" mục 3. POST trên đề PDF + file Q1/Models/MembershipPackage.cs
        public decimal? Price { get; set; }          // [SỬA]: Tên & kiểu | [XEM Ở ĐÂU]: ô "Request body" mục 3. POST trên đề PDF + file Q1/Models/MembershipPackage.cs
    }

    [ApiController]
    public class PackagesController : ControllerBase
    {
        // [SỬA CHỖ NÀY]: Đổi `Prn232PeSu2613Context` -> tên file *Context.cs mới | [XEM Ở ĐÂU]: Thư mục Q1/Models/ sau khi scaffold
        private readonly Prn232PeSu2613Context _context;

        // [SỬA CHỖ NÀY]: Đổi `Prn232PeSu2613Context` khớp dòng trên
        public PackagesController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        // ==================== API 1: GET DANH SÁCH + ĐẾM ====================
        // [SỬA CHỖ NÀY]: Sửa chuỗi trong [HttpGet("...")] | [XEM Ở ĐÂU]: Bảng URL trang 2 đề PDF hoặc tiêu đề "1. GET /api/..."
        [HttpGet("api/MembershipPackages")]
        public IActionResult GetMembershipPackages()
        {
            // [SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages` | [XEM Ở ĐÂU]: Mở file *Context.cs xem tên DbSet / Hoac o trong de thi 1.Get/api/...(VD : MembershipPackages)
            var result = _context.MembershipPackages
                .Select(p => new
                {
                    // [SỬA CHỖ NÀY]: Đổi tên key bên trái + thuộc tính bên phải | [XEM Ở ĐÂU]: Mục "1. GET" trên đề PDF -> phần "Response Fields" và hình đen "Response body 200"; tên cột xem trong file Model
                    PackageId = p.PackageId,
                    PackageName = p.PackageName,
                    Price = p.Price,
                    MemberCount = p.Members.Count    // [SỬA]: Đổi tên key đếm + `p.<TenBangCon>.Count` | [XEM Ở ĐÂU]: tên ICollection cuối file Q1/Models/<TenBang>.cs
                })
                .ToList();

            return Ok(result);
        }

        // ==================== API 2: GET FILTER + TÍNH TOÁN ====================
        // [SỬA CHỖ NÀY]: Sửa route [HttpGet("...")] + tên 2 tham số `(int? packageId, int? minDuration)`
        // [XEM Ở ĐÂU]: Tiêu đề "2. GET /api/..." và phần "Filtering Logic" trên đề PDF hoặc nhìn chữ sau dấu `?` trong ảnh Request URL (vd: ?packageId=1&minDuration=50)
        [HttpGet("api/filter-members")]
        public IActionResult FilterMembers(int? packageId, int? minDuration)
        {
            // [SỬA CHỖ NÀY]: Đổi biến check `< 0` và câu chữ trong BadRequest("...") | [XEM Ở ĐÂU]: Mục "Error handling" của phần "2. GET" & ảnh 400 Bad Request trên đề PDF
            if (minDuration.HasValue && minDuration.Value < 0)
            {
                return BadRequest("minDuration cannot be negative");
            }

            // [SỬA CHỖ NÀY]: Đổi bảng chính & các dòng .Include/.ThenInclude | [XEM Ở ĐÂU]: Mở file Q1/Models/<BangChinh>.cs xem các thuộc tính navigation ở cuối file + sơ đồ bảng trang 2 đề PDF
            var members = _context.Members
                .Include(m => m.Package)
                .Include(m => m.BookingDetails)
                    .ThenInclude(bd => bd.Booking)
                        .ThenInclude(b => b.Trainer)
                .ToList();

            // [SỬA CHỖ NÀY]: Điều kiện lọc tham số 1 | [XEM Ở ĐÂU]: Phần "Filtering Logic" mục 2 trên đề PDF + quan hệ trong file Model (1-N dùng ==, N-N dùng .Any)
            if (packageId.HasValue)
            {
                members = members.Where(m => m.PackageId == packageId.Value).ToList();
            }

            // [SỬA CHỖ NÀY]: Điều kiện lọc tham số 2 (>=) | [XEM Ở ĐÂU]: Phần "Filtering Logic" mục 2 trên đề PDF
            if (minDuration.HasValue)
            {
                members = members.Where(m => m.BookingDetails.Sum(bd => bd.DurationMinutes ?? 0) >= minDuration.Value).ToList();
            }

            var result = members
                // [SỬA CHỖ NÀY]: Cột sort & chiều OrderBy / OrderByDescending | [XEM Ở ĐÂU]: Dòng "- Sorting:" trong phần "2. GET" trên đề PDF
                .OrderByDescending(m => m.JoinDate)
                .Select(m =>
                {
                    // [SỬA CHỖ NÀY]: Bảng chi tiết & cột ngày giờ mới nhất | [XEM Ở ĐÂU]: Mục "Response Fields" phần "2. GET" trên đề PDF (dòng mô tả "most recently based on ...")
                    var details = m.BookingDetails
                        .OrderByDescending(bd => bd.Booking?.SessionTime)
                        .ToList();

                    // [SỬA CHỖ NÀY]: Lấy tên đối tượng gần nhất + chuỗi mặc định trong ngoặc kép | [XEM Ở ĐÂU]: Dòng "return "..." if no ..." mục "Response Fields" phần "2. GET" trên đề PDF
                    var lastBooking = details.FirstOrDefault()?.Booking;
                    string lastTrainer = lastBooking?.Trainer?.TrainerName ?? "No Trainers";

                    // [SỬA CHỖ NÀY]: Nếu cột ngày là `DateOnly?` giữ nguyên dòng dưới; nếu là `DateTime?` đổi thành `m.JoinDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")`
                    // [XEM Ở ĐÂU]: Mở file Q1/Models/<BangChinh>.cs xem kiểu của cột ngày + xem định dạng ngày trong ảnh đen "Response body 200" phần "2. GET"
                    string joinDateStr = m.JoinDate.HasValue
                        ? m.JoinDate.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-ddTHH:mm:ss")
                        : "";

                    return new
                    {
                        // [SỬA CHỖ NÀY]: Tên các key JSON + thuộc tính lấy dữ liệu | [XEM Ở ĐÂU]: Ảnh đen "Response body 200" & mục "Response Fields" phần "2. GET" trên đề PDF
                        MemberId = m.MemberId,
                        FullName = m.FullName,
                        JoinDate = joinDateStr,
                        PackageName = m.Package?.PackageName ?? "",
                        // [SỬA CHỖ NÀY]: Tên key + cột tính Sum | [XEM Ở ĐÂU]: Mô tả "total sum of ..." mục "Response Fields" phần "2. GET"
                        TotalMinutesTrained = details.Sum(bd => bd.DurationMinutes ?? 0),
                        LastTrainerName = lastTrainer
                    };
                })
                .ToList();

            return Ok(result);
        }

        // ==================== API 3: POST THÊM MỚI ====================
        // [SỬA CHỖ NÀY]: Route [HttpPost("...")] | [XEM Ở ĐÂU]: Tiêu đề "3. POST /api/..." trên đề PDF
        [HttpPost("api/MembershipPackages")]
        public IActionResult CreatePackage([FromBody] PackageCreateDto dto)
        {
            // [SỬA CHỖ NÀY]: Đổi `dto.PackageName`, `_context.MembershipPackages`, `p.PackageName` -> cột kiểm tra trùng | [XEM Ở ĐÂU]: Mục "Error handling" của phần "3. POST" trên đề PDF
            string pkgName = (dto.PackageName ?? "").Trim();

            bool exists = _context.MembershipPackages
                .Any(p => p.PackageName != null && p.PackageName.ToLower() == pkgName.ToLower());

            if (exists)
            {
                // [SỬA CHỖ NÀY]: Copy y hệt câu chữ lỗi trùng | [XEM Ở ĐÂU]: Mục "Error handling" phần "3. POST" hoặc ảnh đen 400 Bad Request trên đề PDF
                return BadRequest("Membership package name already exists");
            }

            // [SỬA CHỖ NÀY]: Đổi `new MembershipPackage` -> tên class trong Q1/Models/ và gán đủ các field của `dto`
            var pkg = new MembershipPackage
            {
                PackageName = dto.PackageName,
                Price = dto.Price
            };

            // [SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages` -> tên DbSet trong *Context.cs
            _context.MembershipPackages.Add(pkg);
            _context.SaveChanges();

            return StatusCode(201, pkg);
        }

        // ==================== API 4: DELETE XÓA ====================
        // [SỬA CHỖ NÀY]: Route [HttpDelete("...")] và tên tham số | [XEM Ở ĐÂU]: Tiêu đề "4. DELETE /api/..." trên đề PDF
        [HttpDelete("api/MembershipPackages/{packageId}")]
        public IActionResult DeletePackage(int packageId)
        {
            // [SỬA CHỖ NÀY]: Đổi `_context.MembershipPackages`, `.Include(...)`, và `p.PackageId == packageId` | [XEM Ở ĐÂU]: Mở Q1/Models/<BangBiXoa>.cs xem tên khóa chính & tên các ICollection cần Include
            var pkg = _context.MembershipPackages
                .Include(p => p.Members)
                .FirstOrDefault(p => p.PackageId == packageId);

            if (pkg == null)
            {
                return NotFound();
            }

            // [SỬA CHỖ NÀY]: Đổi `pkg.Members.Any()` -> bảng con bị cấm xóa + câu thông báo lỗi | [XEM Ở ĐÂU]: Mục "Error handling" phần "4. DELETE" & ảnh đen 400 Bad Request trên đề PDF
            if (pkg.Members.Any())
            {
                return BadRequest("Cannot delete membership package with active registered members");
            }

            // [SỬA CHỖ NÀY]: Nếu bảng bị xóa có bảng trung gian N-N thì thêm `pkg.<BangTrungGian>.Clear();` trước Remove; đổi `_context.MembershipPackages` | [XEM Ở ĐÂU]: Sơ đồ DB trang 2 đề PDF & Q1/Models/
            _context.MembershipPackages.Remove(pkg);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
