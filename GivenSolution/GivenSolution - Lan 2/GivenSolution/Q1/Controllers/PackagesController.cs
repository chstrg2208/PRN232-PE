using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    public class PackageCreateDto
    {
        public string? PackageName { get; set; }     
        public decimal? Price { get; set; }         
    }

    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly Prn232PeSu2613Context _context;
        public PackagesController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        [HttpGet("api/MembershipPackages")]
        public IActionResult GetMembershipPackages()
        {       
            var result = _context.MembershipPackages
                .Select(p => new
                {
                    PackageId = p.PackageId,
                    PackageName = p.PackageName,
                    Price = p.Price,
                    MemberCount = p.Members.Count   
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("api/filter-members")]
        public IActionResult FilterMembers(int? packageId, int? minDuration)
        {
            if (minDuration.HasValue && minDuration.Value < 0)
            {
                return BadRequest("minDuration cannot be negative");
            }
            var members = _context.Members
                .Include(m => m.Package)
                .Include(m => m.BookingDetails)
                    .ThenInclude(bd => bd.Booking)
                        .ThenInclude(b => b.Trainer)
                .OrderByDescending(m => m.JoinDate)
                .ToList();

            if (packageId.HasValue)
            {
                members = members.Where(m => m.PackageId == packageId.Value).ToList();
            }

            var result = new List<object>();

            foreach (var m in members)
            {
                // [SỬA CHỖ NÀY]: Bảng chi tiết & cột ngày giờ mới nhất | [XEM Ở ĐÂU]: Mục "Response Fields" phần "2. GET" trên đề PDF (dòng mô tả "most recently based on ...")
                var details = m.BookingDetails
                    .OrderByDescending(bd => bd.Booking?.SessionTime)
                    .ToList();

                // [SỬA CHỖ NÀY]: Cột tính tổng | [XEM Ở ĐÂU]: Mô tả "total sum of ..." mục "Response Fields" phần "2. GET" trên đề PDF
                int totalMinutes = details.Sum(bd => bd.DurationMinutes ?? 0);

                // [SỬA CHỖ NÀY]: Điều kiện lọc tham số 2 | [XEM Ở ĐÂU]: Phần "Filtering Logic" mục 2 trên đề PDF
                if (minDuration.HasValue && totalMinutes < minDuration.Value)
                {
                    continue;
                }

                // [SỬA CHỖ NÀY]: Lấy tên đối tượng gần nhất + chuỗi mặc định khi rỗng | [XEM Ở ĐÂU]: Dòng "return "..." if no ..." mục "Response Fields" phần "2. GET" trên đề PDF
                var lastBooking = details.FirstOrDefault()?.Booking;
                string lastTrainer = lastBooking?.Trainer?.TrainerName ?? "No Trainers";

                // [SỬA CHỖ NÀY]: Nếu cột ngày là `DateOnly?` giữ nguyên dòng dưới; nếu là `DateTime?` đổi thành `m.JoinDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")`
                // [XEM Ở ĐÂU]: Mở file Q1/Models/<BangChinh>.cs xem kiểu của cột ngày + xem định dạng ngày trong ảnh đen "Response body 200" phần "2. GET"
                string joinDateStr = m.JoinDate.HasValue
                    ? m.JoinDate.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-ddTHH:mm:ss")
                    : "";

                result.Add(new
                {
                    // [SỬA CHỖ NÀY]: Tên các key JSON + thuộc tính lấy dữ liệu | [XEM Ở ĐÂU]: Ảnh đen "Response body 200" & mục "Response Fields" phần "2. GET" trên đề PDF
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
