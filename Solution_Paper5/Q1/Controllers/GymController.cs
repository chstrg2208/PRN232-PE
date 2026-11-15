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
    // - XÓA 3 dòng TrainerName, Email, ExperienceYears bên dưới và THAY BẰNG các thuộc tính của đề mới.
    // =========================================================================
    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: DTO hứng dữ liệu JSON khi POST tạo mới
    // -> Mở đề PDF phần 1.3 (Create) xem Request Body JSON có những trường nào thì khai báo đúng tên + kiểu dữ liệu ở đây
    public class TrainerCreateDto
    {
        public string? TrainerName { get; set; }     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên theo field tên trong JSON POST (vd: DoctorName, StudentName...)
        public string? Email { get; set; }           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Field dùng check trùng (Email/Code/Name...)
        public int? ExperienceYears { get; set; }    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Field số/năm/giá trong JSON POST (xóa/thêm tùy đề)
    }

    // =========================================================================
    // STEP 2: KHAI BÁO CONTROLLER VÀ TIÊM DBCONTEXT VÀO (DEPENDENCY INJECTION)
    // =========================================================================
    [ApiController]
    public class GymController : ControllerBase
    {
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // Thay `Prn232PeSu2613Context` ở cả 2 dòng dưới bằng tên class *Context trong thư mục Models/
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Tên class DbContext (xem trong thư mục Models/ hoặc file Program.cs sau khi scaffold)
        private readonly Prn232PeSu2613Context _context;

        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên hàm tạo trùng tên class Controller + đúng kiểu DbContext
        public GymController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        // =====================================================================
        // CÂU 1.1 (1.0 ĐIỂM): GET DANH SÁCH BẢNG DANH MỤC + ĐẾM SỐ LƯỢNG BẢNG CON
        // Đề hiện tại: GET /api/Specializations (Lấy danh sách chuyên môn + đếm số Trainer)
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] cho giống hệt đường dẫn câu 1.1 trong đề.
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Route câu 1.1 - copy đúng path trong đề PDF (vd: "api/Specializations", "api/Departments")
        [HttpGet("api/Specializations")]
        public IActionResult GetSpecializations()
        {
            // STEP 3.1: Truy vấn bảng danh mục từ _context và dùng .Select() tạo đối tượng trả về
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.Specializations` thành DbSet danh mục của đề mới
            var result = _context.Specializations
                .Select(s => new
                {
                    SpecId = s.SpecId,               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Khóa chính bảng danh mục
                    SpecName = s.SpecName,           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Tên danh mục
                    Description = s.Description,     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Cột mô tả (xóa nếu JSON mẫu không có, thêm cột khác nếu đề yêu cầu)
                    TrainerCount = s.Trainers.Count  // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key đếm + Navigation Collection sang bảng con (`s.TenBangCon.Count`)
                })
                .ToList();

            // STEP 3.2: Trả về mã 200 OK kèm danh sách JSON
            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.2 (1.5 ĐIỂM): GET FILTER (LỌC DỮ LIỆU + TÍNH TOÁN + BẮT LỖI 400)
        // Đề hiện tại: GET /api/filter-trainers?specId=1&minExperience=2
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] đúng theo câu 1.2 trong đề PDF.
        // 2. Sửa tên 2 tham số hàm `(int? specId, int? minExperience)` KHỚP 100% tên query param sau `?` trong URL!
        [HttpGet("api/filter-trainers")]
        public IActionResult FilterTrainers(int? specId, int? minExperience)
        {
            // STEP 4.1: Kiểm tra điều kiện số âm -> trả về 400 BadRequest (ăn ngay 0.25 - 0.5 điểm!)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi tên biến cần check âm và copy Y HỆT chuỗi thông báo lỗi trong đề PDF!
            if (minExperience.HasValue && minExperience.Value < 0)
            {
                return BadRequest("minExperience cannot be negative");
            }

            // STEP 4.2: Load toàn bộ bảng chính kèm các bảng liên quan bằng .Include() và .ThenInclude()
            // MẸO ĐI THI: Dùng .ToList() kéo hết lên RAM rồi lọc bằng C# sẽ KHÔNG BAO GIỜ bị lỗi dịch SQL của EF Core!
            var trainers = _context.Trainers                 // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi bảng chính (vd: _context.Doctors, _context.Products...)
                .Include(t => t.Specs)                       // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng danh mục nối N-N hoặc 1-N (mở Models/*.cs xem tên)
                .Include(t => t.Bookings)                    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng giao dịch/lịch đặt
                    .ThenInclude(b => b.BookingDetails)      // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng chi tiết (nếu có; nếu không có thì xóa ThenInclude cấp này)
                        .ThenInclude(d => d.Member)          // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng khách hàng/người dùng
                .ToList();

            // STEP 4.3: Lọc theo tham số thứ 1 (nếu người dùng có truyền lên)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]:
            // - Nếu là quan hệ N-N (1 Trainer có nhiều Specs): dùng `t.Specs.Any(s => s.SpecId == specId.Value)`
            // - Nếu là quan hệ 1-N (có FK trực tiếp): dùng `t.SpecId == specId.Value`
            if (specId.HasValue)
            {
                trainers = trainers.Where(t => t.Specs.Any(s => s.SpecId == specId.Value)).ToList();
            }

            // STEP 4.4: Lọc theo tham số thứ 2 (số năm kinh nghiệm tối thiểu)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Thay `t.ExperienceYears` bằng cột số cần so sánh `>=` (hoặc lọc sau khi tính Sum giống PackagesController)
            if (minExperience.HasValue)
            {
                trainers = trainers.Where(t => t.ExperienceYears >= minExperience.Value).ToList();
            }

            // STEP 4.5: Sắp xếp và gom dữ liệu trả về đúng cấu trúc JSON của đề
            var result = trainers
                .OrderByDescending(t => t.ExperienceYears) // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi cột sắp xếp & hướng tăng (.OrderBy) hay giảm (.OrderByDescending)
                .Select(t =>
                {
                    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Gom chi tiết giao dịch của bản ghi chính, sắp xếp theo thời gian mới nhất đứng đầu
                    var details = t.Bookings
                        .OrderByDescending(b => b.SessionTime)
                        .SelectMany(b => b.BookingDetails)
                        .ToList();

                    // Lấy buổi tập gần nhất (đứng đầu sau khi OrderByDescending)
                    var lastDetail = details.FirstOrDefault();

                    // Trả về đối tượng có tên trường PHẢI KHỚP 100% JSON mẫu trong đề PDF
                    return new
                    {
                        TrainerId = t.TrainerId,             // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Khóa chính
                        TrainerName = t.TrainerName,         // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Tên
                        Email = t.Email,                     // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Cột phụ 1
                        ExperienceYears = t.ExperienceYears, // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + Cột số
                        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + công thức Sum cột thời lượng/tiền (`?? 0` tránh null)
                        TotalMinutesTrained = details.Sum(d => d.DurationMinutes ?? 0),
                        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Xem kỹ đề PDF chuỗi fallback khi rỗng là "No Bookings" hay "No Members" hay "N/A"!
                        LastBookedMember = lastDetail != null ? lastDetail.Member.FullName : "No Bookings",
                        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Key JSON + mảng tên danh mục (`List<string>`)
                        SpecializationList = t.Specs.Select(s => s.SpecName).ToList()
                    };
                })
                .ToList();

            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.3 (1.25 ĐIỂM): POST THÊM MỚI + KIỂM TRA TRÙNG DỮ LIỆU
        // Đề hiện tại: POST /api/trainers
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa đường dẫn trong [HttpPost("...")] đúng theo câu 1.3 của đề PDF
        [HttpPost("api/trainers")]
        public IActionResult CreateTrainer([FromBody] TrainerCreateDto dto)
        {
            // STEP 5.1: Chuẩn hóa trường cần kiểm tra trùng về chữ thường (không phân biệt hoa/thường)
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `dto.Email` thành field cần check trùng (vd: Email, Code, Name, Phone...)
            string email = (dto.Email ?? "").Trim().ToLower();

            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.Trainers` và `t.Email` đúng bảng + cột check trùng
            bool exists = _context.Trainers.Any(t => t.Email != null && t.Email.ToLower() == email);
            if (exists)
            {
                // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Copy Y HỆT câu báo lỗi trùng trong đề PDF dán vào đây!
                return BadRequest("Trainer email already exists");
            }

            // STEP 5.2: Tạo đối tượng Entity mới từ DTO để lưu xuống Database
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `new Trainer` thành tên class Model trong Models/ và map đủ các field POST
            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Map thuộc tính 1 từ dto sang Model
                Email = dto.Email,                           // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Map thuộc tính 2 từ dto sang Model
                ExperienceYears = dto.ExperienceYears        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Map thuộc tính 3 từ dto sang Model
            };

            // STEP 5.3: Lưu vào DB
            _context.Trainers.Add(trainer);                  // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.Trainers` theo tên bảng
            _context.SaveChanges();

            // STEP 5.4: Trả về mã 201 Created kèm đối tượng vừa tạo (ID đã tự động tăng)
            return StatusCode(201, trainer);
        }

        // =====================================================================
        // CÂU 1.4 (1.25 ĐIỂM): DELETE XÓA BẢN GHI + KIỂM TRA RÀNG BUỘC KHÓA NGOẠI
        // Đề hiện tại: DELETE /api/trainers/{trainerId}
        // =====================================================================
        // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Sửa route [HttpDelete("...")] và tên biến `{trainerId}` khớp tham số `int trainerId`
        [HttpDelete("api/trainers/{trainerId}")]
        public IActionResult DeleteTrainer(int trainerId)
        {
            // STEP 6.1: Tìm bản ghi cần xóa, nhớ .Include() các bảng liên kết để kiểm tra hoặc xóa kèm
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.Trainers`, `.Include(...)`, và điều kiện khóa chính `t.TrainerId == trainerId`
            var trainer = _context.Trainers
                .Include(t => t.Specs)       // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng trung gian N-N (cần .Clear() trước khi Remove nếu có!)
                .Include(t => t.Bookings)    // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Bảng con giao dịch (nếu có dữ liệu thì cấm xóa!)
                .FirstOrDefault(t => t.TrainerId == trainerId);

            // STEP 6.2: Nếu không tìm thấy ID -> Trả về 404 NotFound (ăn 0.25 điểm)
            if (trainer == null)
            {
                return NotFound();
            }

            // STEP 6.3: Nếu đang có dữ liệu ở bảng giao dịch (Bookings) -> Cấm xóa, trả về 400 BadRequest
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `trainer.Bookings.Any()` thành bảng con cấm xóa, và copy Y HỆT câu báo lỗi trong đề PDF!
            if (trainer.Bookings.Any())
            {
                return BadRequest("Cannot delete trainer with active booking records");
            }

            // STEP 6.4: Xóa dữ liệu ở bảng trung gian (nếu có quan hệ N-N) rồi xóa bản ghi chính
            // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Nếu bảng bị xóa KHÔNG có bảng trung gian N-N thì XÓA dòng `trainer.Specs.Clear();` đi!
            trainer.Specs.Clear();
            _context.Trainers.Remove(trainer);               // 🔧 [ĐỔI ĐỀ SỬA CHỖ NÀY]: Đổi `_context.Trainers` theo tên bảng
            _context.SaveChanges();

            // STEP 6.5: Xóa thành công -> Trả về 204 NoContent (theo chuẩn REST của đề)
            return NoContent();
        }
    }
}
