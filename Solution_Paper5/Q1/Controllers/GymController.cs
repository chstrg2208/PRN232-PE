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
    public class TrainerCreateDto
    {
        public string? TrainerName { get; set; }     // <-- [SỬA KHI ĐỔI ĐỀ]: Tên thuộc tính chuỗi 1 trong JSON POST
        public string? Email { get; set; }           // <-- [SỬA KHI ĐỔI ĐỀ]: Tên thuộc tính chuỗi 2 (thường dùng để check trùng)
        public int? ExperienceYears { get; set; }    // <-- [SỬA KHI ĐỔI ĐỀ]: Tên thuộc tính số trong JSON POST
    }

    // =========================================================================
    // STEP 2: KHAI BÁO CONTROLLER VÀ TIÊM DBCONTEXT VÀO (DEPENDENCY INJECTION)
    // =========================================================================
    [ApiController]
    public class GymController : ControllerBase
    {
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // Thay `Prn232PeSu2613Context` ở cả 2 dòng dưới bằng tên class *Context trong thư mục Models/
        private readonly Prn232PeSu2613Context _context;

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
        [HttpGet("api/Specializations")]
        public IActionResult GetSpecializations()
        {
            // STEP 3.1: Truy vấn bảng danh mục từ _context và dùng .Select() tạo đối tượng trả về
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Thay `_context.Specializations` bằng bảng danh mục của đề (VD: _context.Departments, _context.Categories...)
            // - Sửa tên các thuộc tính bên trong `new { ... }` khớp với JSON mẫu trong đề bài!
            // - Chỗ `.Count`: lấy danh sách bảng liên quan chấm `.Count` (VD: s.Doctors.Count, s.Books.Count)
            var result = _context.Specializations
                .Select(s => new
                {
                    SpecId = s.SpecId,               // <-- [SỬA KHI ĐỔI ĐỀ]: Khóa chính bảng danh mục
                    SpecName = s.SpecName,           // <-- [SỬA KHI ĐỔI ĐỀ]: Tên danh mục
                    Description = s.Description,     // <-- [SỬA KHI ĐỔI ĐỀ]: Mô tả (xóa nếu đề không yêu cầu)
                    TrainerCount = s.Trainers.Count  // <-- [SỬA KHI ĐỔI ĐỀ]: Đếm số phần tử bảng con liên kết
                })
                .ToList();

            // STEP 3.2: Trả về mã 200 OK kèm danh sách JSON
            return Ok(result);
        }

        // =====================================================================
        // CÂU 1.2 (1.5 ĐIỂM): GET FILTER (LỌC DỮ LIỆU + TÍNH TOÁN + BẮT LỖI 400)
        // Đề hiện tại: GET /api/filter-trainers?specId=1&minExperience=2
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpGet("...")] đúng theo câu 1.2 của đề.
        // 2. Sửa tên 2 tham số trên hàm `FilterTrainers(int? specId, int? minExperience)`
        //    cho TRÙNG KHỚP 100% tên biến sau dấu `?` trên URL của đề bài!
        [HttpGet("api/filter-trainers")]
        public IActionResult FilterTrainers(int? specId, int? minExperience)
        {
            // STEP 4.1: Kiểm tra điều kiện số âm -> trả về 400 BadRequest (ăn ngay 0.25 - 0.5 điểm!)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Copy Y HỆT câu thông báo lỗi trong đề bài dán vào trong `BadRequest("...")`
            if (minExperience.HasValue && minExperience.Value < 0)
            {
                return BadRequest("minExperience cannot be negative");
            }

            // STEP 4.2: Load toàn bộ bảng chính kèm các bảng liên quan bằng .Include() và .ThenInclude()
            // MẸO ĐI THI: Dùng .ToList() kéo hết lên RAM rồi lọc bằng C# sẽ KHÔNG BAO GIỜ bị lỗi dịch SQL của EF Core!
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Mở file class chính trong Models/ (ở đây là Trainer.cs) xem bên dưới cùng có những ICollection hay bảng nào
            // thì .Include() hết vào đây!
            var trainers = _context.Trainers
                .Include(t => t.Specs)                       // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng danh mục nối N-N hoặc 1-N
                .Include(t => t.Bookings)                    // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng giao dịch/lịch đặt
                    .ThenInclude(b => b.BookingDetails)      // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng chi tiết (nếu có)
                        .ThenInclude(d => d.Member)          // <-- [SỬA KHI ĐỔI ĐỀ]: Bảng khách hàng/người dùng
                .ToList();

            // STEP 4.3: Lọc theo tham số thứ 1 (nếu người dùng có truyền lên)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Nếu là quan hệ N-N (1 Trainer có nhiều Specs): dùng `t.Specs.Any(s => s.SpecId == ...)`
            // - Nếu là quan hệ 1-N (chỉ có 1 khóa ngoại trực tiếp): dùng `t.SpecId == specId.Value`
            if (specId.HasValue)
            {
                trainers = trainers.Where(t => t.Specs.Any(s => s.SpecId == specId.Value)).ToList();
            }

            // STEP 4.4: Lọc theo tham số thứ 2 (số năm kinh nghiệm tối thiểu)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]: Thay `t.ExperienceYears` bằng cột số cần so sánh `>=`
            if (minExperience.HasValue)
            {
                trainers = trainers.Where(t => t.ExperienceYears >= minExperience.Value).ToList();
            }

            // STEP 4.5: Sắp xếp và gom dữ liệu trả về đúng cấu trúc JSON của đề
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // - Xem đề yêu cầu sắp xếp tăng dần (.OrderBy) hay giảm dần (.OrderByDescending) theo cột nào.
            // - Xem JSON mẫu của đề cần tính tổng (Sum), đếm (Count), hay lấy phần tử mới nhất (FirstOrDefault).
            var result = trainers
                .OrderByDescending(t => t.ExperienceYears) // <-- [SỬA KHI ĐỔI ĐỀ]: Cột cần sắp xếp giảm dần
                .Select(t =>
                {
                    // Gom tất cả chi tiết buổi tập của HLV này, sắp xếp theo thời gian mới nhất đứng đầu
                    var details = t.Bookings
                        .OrderByDescending(b => b.SessionTime)
                        .SelectMany(b => b.BookingDetails)
                        .ToList();

                    // Lấy buổi tập gần nhất (đứng đầu sau khi OrderByDescending)
                    var lastDetail = details.FirstOrDefault();

                    // Trả về đối tượng có tên trường giống hệt JSON mẫu trong đề
                    return new
                    {
                        TrainerId = t.TrainerId,
                        TrainerName = t.TrainerName,
                        Email = t.Email,
                        ExperienceYears = t.ExperienceYears,
                        // Tính tổng số phút tập (nếu null thì tính là 0)
                        TotalMinutesTrained = details.Sum(d => d.DurationMinutes ?? 0),
                        // Lấy tên hội viên đặt gần nhất; nếu chưa có buổi tập nào thì trả về "No Bookings"
                        LastBookedMember = lastDetail != null ? lastDetail.Member.FullName : "No Bookings",
                        // Lấy danh sách tên các chuyên môn kiểu List<string>
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
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpPost("...")] theo đúng đề.
        [HttpPost("api/trainers")]
        public IActionResult CreateTrainer([FromBody] TrainerCreateDto dto)
        {
            // STEP 5.1: Chuẩn hóa trường cần kiểm tra trùng về chữ thường (không phân biệt hoa/thường)
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Xem đề bắt kiểm tra trùng cột nào (Email, Code, Name, Phone...) thì thay `dto.Email` và `t.Email`
            string email = (dto.Email ?? "").ToLower();

            bool exists = _context.Trainers.Any(t => t.Email != null && t.Email.ToLower() == email);
            if (exists)
            {
                // ⚠️ [NẾU ĐỔI ĐỀ]: Copy đúng câu báo lỗi trùng trong đề dán vào đây!
                return BadRequest("Trainer email already exists");
            }

            // STEP 5.2: Tạo đối tượng Entity mới từ DTO để lưu xuống Database
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `new Trainer` bằng class Model cần thêm mới, và gán các thuộc tính từ `dto` sang.
            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,
                Email = dto.Email,
                ExperienceYears = dto.ExperienceYears
            };

            // STEP 5.3: Lưu vào DB
            _context.Trainers.Add(trainer);
            _context.SaveChanges();

            // STEP 5.4: Trả về mã 201 Created kèm đối tượng vừa tạo (ID đã tự động tăng)
            return StatusCode(201, trainer);
        }

        // =====================================================================
        // CÂU 1.4 (1.25 ĐIỂM): DELETE XÓA BẢN GHI + KIỂM TRA RÀNG BUỘC KHÓA NGOẠI
        // Đề hiện tại: DELETE /api/trainers/{trainerId}
        // =====================================================================
        // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
        // 1. Sửa đường dẫn trong [HttpDelete("...")] và tên tham số `int trainerId` khớp với đề.
        [HttpDelete("api/trainers/{trainerId}")]
        public IActionResult DeleteTrainer(int trainerId)
        {
            // STEP 6.1: Tìm bản ghi cần xóa, nhớ .Include() các bảng liên kết để kiểm tra hoặc xóa kèm
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `_context.Trainers`, `.Include(...)` và điều kiện khóa chính `t.TrainerId == trainerId`
            var trainer = _context.Trainers
                .Include(t => t.Specs)       // Bảng trung gian N-N (cần xóa kèm khi xóa Trainer)
                .Include(t => t.Bookings)    // Bảng giao dịch (nếu có dữ liệu thì cấm xóa!)
                .FirstOrDefault(t => t.TrainerId == trainerId);

            // STEP 6.2: Nếu không tìm thấy ID -> Trả về 404 NotFound (ăn 0.25 điểm)
            if (trainer == null)
            {
                return NotFound();
            }

            // STEP 6.3: Nếu đang có dữ liệu ở bảng giao dịch (Bookings) -> Cấm xóa, trả về 400 BadRequest
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Thay `trainer.Bookings.Any()` bằng bảng mà đề cấm xóa khi đang có dữ liệu,
            // và copy Y HỆT câu báo lỗi trong đề vào `BadRequest("...")`
            if (trainer.Bookings.Any())
            {
                return BadRequest("Cannot delete trainer with active booking records");
            }

            // STEP 6.4: Xóa dữ liệu ở bảng trung gian (nếu có quan hệ N-N) rồi xóa bản ghi chính
            // ⚠️ [NẾU ĐỔI ĐỀ - SỬA Ở ĐÂY]:
            // Nếu bảng cần xóa không có bảng trung gian N-N thì XÓA dòng `trainer.Specs.Clear();` đi!
            trainer.Specs.Clear();
            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            // STEP 6.5: Xóa thành công -> Trả về 204 NoContent (theo chuẩn REST của đề)
            return NoContent();
        }
    }
}
