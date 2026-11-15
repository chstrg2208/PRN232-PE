using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    // [SỬA CHỖ NÀY]: Xóa/thêm/đổi các dòng bên trong class cho khớp JSON gửi lên
    // [XEM Ở ĐÂU]: Đề PDF Q1 mục "3. POST /api/..." -> nhìn hình ô "Request body" màu xanh lá & dòng "Accept ... inside the request body"; kiểu dữ liệu copy từ Q1/Models/*.cs
    public class TrainerCreateDto
    {
        public string? TrainerName { get; set; }     // [SỬA]: Tên & kiểu | [XEM Ở ĐÂU]: ô "Request body" mục 3. POST trên đề PDF + file Q1/Models/Trainer.cs
        public string? Email { get; set; }           // [SỬA]: Tên & kiểu | [XEM Ở ĐÂU]: ô "Request body" mục 3. POST trên đề PDF + file Q1/Models/Trainer.cs
        public int? ExperienceYears { get; set; }    // [SỬA]: Tên & kiểu | [XEM Ở ĐÂU]: ô "Request body" mục 3. POST trên đề PDF + file Q1/Models/Trainer.cs
    }

    [ApiController]
    public class GymController : ControllerBase
    {
        // [SỬA CHỖ NÀY]: Đổi `Prn232PeSu2613Context` -> tên file *Context.cs mới | [XEM Ở ĐÂU]: Thư mục Q1/Models/ sau khi scaffold
        private readonly Prn232PeSu2613Context _context;

        // [SỬA CHỖ NÀY]: Đổi `Prn232PeSu2613Context` khớp dòng trên
        public GymController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        // ==================== API 1: GET DANH SÁCH + ĐẾM ====================
        // [SỬA CHỖ NÀY]: Sửa chuỗi trong [HttpGet("...")] | [XEM Ở ĐÂU]: Bảng URL trang 2 đề PDF hoặc tiêu đề "1. GET /api/..."
        [HttpGet("api/Specializations")]
        public IActionResult GetSpecializations()
        {
            // [SỬA CHỖ NÀY]: Đổi `_context.Specializations` | [XEM Ở ĐÂU]: Mở file *Context.cs xem tên DbSet
            var result = _context.Specializations
                .Select(s => new
                {
                    // [SỬA CHỖ NÀY]: Đổi tên key bên trái + thuộc tính bên phải | [XEM Ở ĐÂU]: Mục "1. GET" trên đề PDF -> phần "Response Fields" và hình đen "Response body 200"; tên cột xem trong file Model
                    SpecId = s.SpecId,
                    SpecName = s.SpecName,
                    Description = s.Description,
                    TrainerCount = s.Trainers.Count  // [SỬA]: Đổi tên key đếm + `s.<TenBangCon>.Count` | [XEM Ở ĐÂU]: tên ICollection cuối file Q1/Models/<TenBang>.cs
                })
                .ToList();

            return Ok(result);
        }

        // ==================== API 2: GET FILTER + TÍNH TOÁN ====================
        // [SỬA CHỖ NÀY]: Sửa route [HttpGet("...")] + tên 2 tham số `(int? specId, int? minExperience)`
        // [XEM Ở ĐÂU]: Tiêu đề "2. GET /api/..." và phần "Filtering Logic" trên đề PDF hoặc nhìn chữ sau dấu `?` trong ảnh Request URL (vd: ?specId=1&minExperience=2)
        [HttpGet("api/filter-trainers")]
        public IActionResult FilterTrainers(int? specId, int? minExperience)
        {
            // [SỬA CHỖ NÀY]: Đổi biến check `< 0` và câu chữ trong BadRequest("...") | [XEM Ở ĐÂU]: Mục "Error handling" của phần "2. GET" & ảnh 400 Bad Request trên đề PDF
            if (minExperience.HasValue && minExperience.Value < 0)
            {
                return BadRequest("minExperience cannot be negative");
            }

            // [SỬA CHỖ NÀY]: Đổi bảng chính & các dòng .Include/.ThenInclude | [XEM Ở ĐÂU]: Mở file Q1/Models/<BangChinh>.cs xem các thuộc tính navigation ở cuối file + sơ đồ bảng trang 2 đề PDF
            var trainers = _context.Trainers
                .Include(t => t.Specs)
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(d => d.Member)
                .ToList();

            // [SỬA CHỖ NÀY]: Điều kiện lọc tham số 1 | [XEM Ở ĐÂU]: Phần "Filtering Logic" mục 2 trên đề PDF + quan hệ trong file Model (N-N dùng .Any, 1-N dùng ==)
            if (specId.HasValue)
            {
                trainers = trainers.Where(t => t.Specs.Any(s => s.SpecId == specId.Value)).ToList();
            }

            // [SỬA CHỖ NÀY]: Điều kiện lọc tham số 2 (>=) | [XEM Ở ĐÂU]: Phần "Filtering Logic" mục 2 trên đề PDF
            if (minExperience.HasValue)
            {
                trainers = trainers.Where(t => t.ExperienceYears >= minExperience.Value).ToList();
            }

            var result = trainers
                // [SỬA CHỖ NÀY]: Cột sort & chiều OrderBy / OrderByDescending | [XEM Ở ĐÂU]: Dòng "- Sorting:" trong phần "2. GET" trên đề PDF
                .OrderByDescending(t => t.ExperienceYears)
                .Select(t =>
                {
                    // [SỬA CHỖ NÀY]: Bảng giao dịch & cột ngày giờ mới nhất | [XEM Ở ĐÂU]: Mục "Response Fields" phần "2. GET" trên đề PDF (dòng mô tả "most recently based on ...")
                    var details = t.Bookings
                        .OrderByDescending(b => b.SessionTime)
                        .SelectMany(b => b.BookingDetails)
                        .ToList();

                    var lastDetail = details.FirstOrDefault();

                    return new
                    {
                        // [SỬA CHỖ NÀY]: Tên các key JSON + thuộc tính lấy dữ liệu | [XEM Ở ĐÂU]: Ảnh đen "Response body 200" & mục "Response Fields" phần "2. GET" trên đề PDF
                        TrainerId = t.TrainerId,
                        TrainerName = t.TrainerName,
                        Email = t.Email,
                        ExperienceYears = t.ExperienceYears,
                        // [SỬA CHỖ NÀY]: Tên key + cột tính Sum | [XEM Ở ĐÂU]: Mô tả "total sum of ..." mục "Response Fields" phần "2. GET"
                        TotalMinutesTrained = details.Sum(d => d.DurationMinutes ?? 0),
                        // [SỬA CHỖ NÀY]: Tên key + chuỗi mặc định trong ngoặc kép | [XEM Ở ĐÂU]: Dòng "return "..." if no bookings" mục "Response Fields" phần "2. GET"
                        LastBookedMember = lastDetail != null ? lastDetail.Member.FullName : "No Bookings",
                        // [SỬA CHỖ NÀY]: Tên key + cột tên trong bảng danh mục | [XEM Ở ĐÂU]: Ảnh đen "Response body 200" phần "2. GET"
                        SpecializationList = t.Specs.Select(s => s.SpecName).ToList()
                    };
                })
                .ToList();

            return Ok(result);
        }

        // ==================== API 3: POST THÊM MỚI ====================
        // [SỬA CHỖ NÀY]: Route [HttpPost("...")] | [XEM Ở ĐÂU]: Tiêu đề "3. POST /api/..." trên đề PDF
        [HttpPost("api/trainers")]
        public IActionResult CreateTrainer([FromBody] TrainerCreateDto dto)
        {
            // [SỬA CHỖ NÀY]: Đổi `dto.Email`, `_context.Trainers`, `t.Email` -> cột kiểm tra trùng | [XEM Ở ĐÂU]: Mục "Error handling" của phần "3. POST" trên đề PDF
            string email = (dto.Email ?? "").Trim().ToLower();

            bool exists = _context.Trainers.Any(t => t.Email != null && t.Email.ToLower() == email);
            if (exists)
            {
                // [SỬA CHỖ NÀY]: Copy y hệt câu chữ lỗi trùng | [XEM Ở ĐÂU]: Mục "Error handling" phần "3. POST" hoặc ảnh đen 400 Bad Request trên đề PDF
                return BadRequest("Trainer email already exists");
            }

            // [SỬA CHỖ NÀY]: Đổi `new Trainer` -> tên class trong Q1/Models/ và gán đủ các field của `dto`
            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,
                Email = dto.Email,
                ExperienceYears = dto.ExperienceYears
            };

            // [SỬA CHỖ NÀY]: Đổi `_context.Trainers` -> tên DbSet trong *Context.cs
            _context.Trainers.Add(trainer);
            _context.SaveChanges();

            return StatusCode(201, trainer);
        }

        // ==================== API 4: DELETE XÓA ====================
        // [SỬA CHỖ NÀY]: Route [HttpDelete("...")] và tên tham số | [XEM Ở ĐÂU]: Tiêu đề "4. DELETE /api/..." trên đề PDF
        [HttpDelete("api/trainers/{trainerId}")]
        public IActionResult DeleteTrainer(int trainerId)
        {
            // [SỬA CHỖ NÀY]: Đổi `_context.Trainers`, các dòng `.Include(...)`, và `t.TrainerId == trainerId` | [XEM Ở ĐÂU]: Mở Q1/Models/<BangBiXoa>.cs xem tên khóa chính & tên các ICollection cần Include
            var trainer = _context.Trainers
                .Include(t => t.Specs)
                .Include(t => t.Bookings)
                .FirstOrDefault(t => t.TrainerId == trainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            // [SỬA CHỖ NÀY]: Đổi `trainer.Bookings.Any()` -> bảng con bị cấm xóa + câu thông báo lỗi | [XEM Ở ĐÂU]: Mục "Error handling" phần "4. DELETE" & ảnh đen 400 Bad Request trên đề PDF
            if (trainer.Bookings.Any())
            {
                return BadRequest("Cannot delete trainer with active booking records");
            }

            // [SỬA CHỖ NÀY]: Nếu bảng bị xóa KHÔNG có bảng trung gian N-N thì XÓA dòng `.Clear()` bên dưới; đổi `_context.Trainers` | [XEM Ở ĐÂU]: Sơ đồ DB trang 2 đề PDF & Q1/Models/
            trainer.Specs.Clear();
            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
