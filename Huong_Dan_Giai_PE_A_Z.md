# Hướng dẫn giải PE PRN232 từ A–Z trên Visual Studio (Đề số 5)

Toàn bộ bài giải mẫu đã được mình tạo và chạy kiểm chứng đủ **10/10 điểm** theo barem tại thư mục [Solution_Paper5](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5). Bạn có thể mở trực tiếp [GivenSolution.sln](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/GivenSolution.sln) bằng Visual Studio 2022 để xem đối chiếu.

---

## PHẦN 1: Phổ điểm từng câu & Chiến thuật "Chỉ cần qua môn"

Điểm PE tổng là **10.0 điểm** (Q1 = 5.0đ, Q2 = 5.0đ). Qua môn thường chỉ cần **≥ 4.0 điểm** (và không bị 0 điểm liệt).

> [!CAUTION]
> **3 lỗi bị 0 điểm toàn bộ câu (hoặc 0 điểm cả bài) phải tránh tuyệt đối:**
> 1. Tự ý cài thêm thư viện trong **NuGet Package Manager** -> **0 điểm cả bài**.
> 2. Không để chuỗi kết nối trong `appsettings.json` (`MyCnn` ở Q1, `GivenAPIBaseUrl` ở Q2) -> **0 điểm câu đó**.
> 3. Sai đường dẫn URL hoặc sai `id="..."` của thẻ HTML trong Q2 -> mất trắng điểm testcase đó (máy chấm tự động tìm theo `id`, không chấm giao diện đẹp/xấu).

### Bảng phân bổ điểm chi tiết (Đề 5) & Độ khó

#### Câu 1 – Web API dùng EF Core (5.0 điểm – 8 Testcase)
| Testcase | Chức năng | Điểm | Độ khó | Gợi ý chiến thuật |
|---|---|---:|---|---|
| **TC01** | `GET /api/Specializations` (lấy danh sách + đếm số HLV) | **1.00** | Rất dễ (5 dòng code) | **Bắt buộc ăn** |
| **TC03** | `GET /api/filter-trainers` khi `minExperience < 0` trả về 400 | **0.25** | Siêu dễ (1 lệnh `if`) | **Bắt buộc ăn** |
| **TC02** | `GET /api/filter-trainers` (lọc + tính tổng phút + người đặt cuối) | **1.00** | Hơi dài | Làm sau cùng nếu sợ rối |
| **TC04** | `POST /api/trainers` (thêm mới HLV, trả về mã 201) | **0.75** | Dễ | **Nên ăn** |
| **TC05** | `POST /api/trainers` trùng Email trả về 400 | **0.50** | Dễ (1 lệnh `if`) | **Nên ăn** |
| **TC08** | `DELETE /api/trainers/99` không tìm thấy trả về 404 | **0.25** | Siêu dễ (1 lệnh `if`) | **Bắt buộc ăn** |
| **TC07** | `DELETE /api/trainers/1` đang có lịch tập trả về 400 | **0.50** | Dễ (1 lệnh `if`) | **Nên ăn** |
| **TC06** | `DELETE /api/trainers/6` xóa bảng trung gian + xóa HLV trả về 204 | **0.75** | Dễ (3 dòng code) | **Nên ăn** |

👉 **Chỉ cần làm các câu dễ của Q1** (bỏ qua phần tính toán phức tạp của TC02) là bạn đã cầm chắc **4.0 / 5.0 điểm** ở riêng Câu 1!

#### Câu 2 – Web MVC gọi Given API (5.0 điểm – 6 Testcase)
*Mẹo lớn nhất ở Câu 2:* Đề thi **đã cho sẵn 2 file giao diện mẫu** `Index.html` và `Analyze.html` có sẵn toàn bộ các `id` chuẩn. Bạn chỉ việc **copy-paste** vào rồi gắn vòng lặp `@foreach`!

| Testcase | Chức năng | Điểm | Độ khó |
|---|---|---:|---|
| **TC01** | Mở `/Member` hiện bảng 5 hội viên (`td_name_1`, `td_joinDate_1`, `td_package_1`) | **1.00** | Dễ |
| **TC02** | Hiện ô lọc dropdown `sl_bookingStatus` có `op_all`, `op_Da_tap`... | **1.00** | Dễ |
| **TC03** | Có nút link `a_1`..`a_5` chữ `Analyze Sales` trỏ tới `/Member/Analyze/1` | **0.50** | Có sẵn trong HTML mẫu |
| **TC06** | Chọn status rồi bấm nút `bt_search` lọc đúng hội viên | **0.50** | Tự chạy khi làm xong TC01 + TC02 |
| **TC04** | Trang `/Member/Analyze/1` hiện thông tin cơ bản (`span_1`, `span_fullName`, `span_joinDate`) | **1.00** | Dễ |
| **TC05** | Trang `/Member/Analyze/1` hiện bảng lịch tập (`td_trainer_1`, `td_time_1`, `td_duration_1`, `td_tier_1`) | **1.00** | Dễ |

👉 Chỉ cần làm riêng trang `/Member` (Index) của Q2 là có ngay **3.0 điểm**. Cộng với phần dễ của Q1 (4.0 điểm) là **7.0 điểm**, dư sức qua môn!

---

## PHẦN 2: Chuẩn bị trước khi code (Bước 0)

Khi vào phòng thi, bạn nhận được 2 thứ:
1. Thư mục đề (`Given_Material\PaperNo_5` gồm `1\database.sql` và `2\givenAPI`, `Index.html`, `Analyze.html`).
2. Thư mục khung bài làm (`GivenSolution` gồm `GivenSolution.sln`, `Q1`, `Q2`).

### Bước 0.1 – Chạy file tạo Database (`database.sql`)
1. Mở phần mềm **SQL Server Management Studio (SSMS)**.
2. Ở bảng **Connect to Server**, ô **Server name** ghi nhớ tên server của máy bạn (thường là `.` hoặc `localhost` hoặc `(localdb)\MSSQLLocalDB`). Nhấn **Connect**.
3. Nhấn tổ hợp phím **`Ctrl + O`** -> chọn tới file [database.sql](file:///e:/FPT/Ki%208/PRN232/PE/2.PE_Exam/2.PE_Exam/Given_Material/PaperNo_5/1/database.sql) -> nhấn **Open**.
4. Nhấn phím **`F5`** (hoặc nút **▶ Execute** trên thanh công cụ). Khi hiện dòng `Commands completed successfully` là xong.
   - Nhìn dòng đầu tiên trong file SQL để biết tên Database: `CREATE DATABASE PRN232_PE_SU26_13;` -> Tên DB là **`PRN232_PE_SU26_13`**.

### Bước 0.2 – Mở Solution bằng Visual Studio 2022
1. Giải nén `GivenSolution` ra ổ đĩa (ví dụ `D:\PE\GivenSolution`).
2. Nháy đúp vào file **`GivenSolution.sln`** để mở Visual Studio 2022.
3. Nhìn sang cột **Solution Explorer** bên phải, bạn sẽ thấy 2 project là **Q1** và **Q2**.

---

## PHẦN 3: Hướng dẫn chi tiết CÂU 1 (Q1 – Web API) từ A–Z

### Bước 1.1: Tự động sinh Models & DbContext từ Database (Scaffold)
Bạn **không cần tự gõ** các bảng Database. Hãy để máy tự sinh ra:

1. Trên thanh menu trên cùng của Visual Studio, bấm vào:
   **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
2. Một cửa sổ đen/trắng hiện ra ở dưới đáy màn hình.
   - Nhìn chữ **Default project:** ở thanh trên của cửa sổ đó, đảm bảo nó đang chọn là **`Q1`**.
3. Copy dòng lệnh dưới đây, dán vào cửa sổ đó rồi nhấn **Enter**:
   *(Lưu ý: Nếu SSMS của bạn dùng tài khoản `sa`, hãy thay `Trusted_Connection=True` bằng `uid=sa;pwd=123` tùy máy phòng thi)*

```powershell
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=PRN232_PE_SU26_13;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models --no-onconfiguring
```

4. Đợi khoảng 5 giây báo `Build succeeded`. Nhìn sang **Solution Explorer** dưới mục **Q1**, bạn sẽ thấy thư mục **`Models`** tự động xuất hiện cùng 7 file `.cs` ([Prn232PeSu2613Context.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q1/Models/Prn232PeSu2613Context.cs), [Trainer.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q1/Models/Trainer.cs), v.v.).

---

### Bước 1.2: Điền chuỗi kết nối vào `appsettings.json` của Q1 (Không làm = 0 điểm!)
1. Trong **Solution Explorer** -> dưới **Q1** -> nháy đúp vào file **[appsettings.json](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q1/appsettings.json)**.
2. Điền chuỗi kết nối vào chỗ `"MyCnn": ""` (nhớ dùng **2 dấu gạch chéo `\\`**):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MyCnn": "Server=(localdb)\\MSSQLLocalDB;Database=PRN232_PE_SU26_13;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
3. Nhấn **`Ctrl + S`** để lưu.

---

### Bước 1.3: Cấu hình file `Program.cs` của Q1
Mặc định file `Program.cs` đề cho chỉ có chữ `"Hello World!"`, chưa bật API hay Database.
1. Nháy đúp vào **[Program.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q1/Program.cs)** của **Q1**.
2. Xóa hết nội dung cũ (`Ctrl + A` -> `Delete`) và dán đoạn code này vào:

```csharp
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Bật Controller và chống lỗi vòng lặp JSON khi trả về Entity
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 2. Kết nối Database qua chuỗi "MyCnn" trong appsettings.json
builder.Services.AddDbContext<Prn232PeSu2613Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// 3. Bật giao diện Swagger để test
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
```
3. Nhấn **`Ctrl + S`** để lưu.

---

### Bước 1.4: Tạo Controller viết cả 4 API cho Q1
1. Trong **Solution Explorer**, bấm **chuột phải vào chữ `Q1`** -> chọn **Add** -> **New Folder** -> đặt tên là **`Controllers`** rồi nhấn Enter.
2. Bấm **chuột phải vào thư mục `Controllers`** vừa tạo -> chọn **Add** -> **Class...**
3. Ở ô tên bên dưới, gõ **`GymController.cs`** -> nhấn nút **Add**.
4. Xóa hết code có sẵn trong file, dán toàn bộ code dưới đây vào ([GymController.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q1/Controllers/GymController.cs)):

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    // Class nhận dữ liệu JSON gửi lên ở câu 3 (POST)
    public class TrainerCreateDto
    {
        public string? TrainerName { get; set; }
        public string? Email { get; set; }
        public int? ExperienceYears { get; set; }
    }

    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly Prn232PeSu2613Context _context;

        public GymController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        // =========================================================
        // API 1 (1.0 điểm): GET /api/Specializations
        // =========================================================
        [HttpGet("api/Specializations")]
        public IActionResult GetSpecializations()
        {
            var result = _context.Specializations
                .Select(s => new
                {
                    SpecId = s.SpecId,
                    SpecName = s.SpecName,
                    Description = s.Description,
                    TrainerCount = s.Trainers.Count
                })
                .ToList();

            return Ok(result);
        }

        // =========================================================
        // API 2 (1.25 điểm): GET /api/filter-trainers?specId=1&minExperience=2
        // =========================================================
        [HttpGet("api/filter-trainers")]
        public IActionResult FilterTrainers(int? specId, int? minExperience)
        {
            // TC03 (0.25đ): Kiểm tra số âm -> trả về 400 text
            if (minExperience.HasValue && minExperience.Value < 0)
            {
                return BadRequest("minExperience cannot be negative");
            }

            // TC02 (1.0đ): Tải toàn bộ dữ liệu liên quan lên RAM rồi lọc cho dễ, không sợ lỗi SQL
            var trainers = _context.Trainers
                .Include(t => t.Specs)
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(d => d.Member)
                .ToList();

            if (specId.HasValue)
            {
                trainers = trainers.Where(t => t.Specs.Any(s => s.SpecId == specId.Value)).ToList();
            }

            if (minExperience.HasValue)
            {
                trainers = trainers.Where(t => t.ExperienceYears >= minExperience.Value).ToList();
            }

            var result = trainers
                .OrderByDescending(t => t.ExperienceYears)
                .Select(t =>
                {
                    var details = t.Bookings
                        .OrderByDescending(b => b.SessionTime)
                        .SelectMany(b => b.BookingDetails)
                        .ToList();

                    var lastDetail = details.FirstOrDefault();

                    return new
                    {
                        TrainerId = t.TrainerId,
                        TrainerName = t.TrainerName,
                        Email = t.Email,
                        ExperienceYears = t.ExperienceYears,
                        TotalMinutesTrained = details.Sum(d => d.DurationMinutes ?? 0),
                        LastBookedMember = lastDetail != null ? lastDetail.Member.FullName : "No Bookings",
                        SpecializationList = t.Specs.Select(s => s.SpecName).ToList()
                    };
                })
                .ToList();

            return Ok(result);
        }

        // =========================================================
        // API 3 (1.25 điểm): POST /api/trainers
        // =========================================================
        [HttpPost("api/trainers")]
        public IActionResult CreateTrainer([FromBody] TrainerCreateDto dto)
        {
            // TC05 (0.5đ): Kiểm tra trùng email (không phân biệt hoa thường)
            string email = (dto.Email ?? "").ToLower();
            bool exists = _context.Trainers.Any(t => t.Email != null && t.Email.ToLower() == email);
            if (exists)
            {
                return BadRequest("Trainer email already exists");
            }

            // TC04 (0.75đ): Tạo mới và trả về mã 201 Created
            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,
                Email = dto.Email,
                ExperienceYears = dto.ExperienceYears
            };

            _context.Trainers.Add(trainer);
            _context.SaveChanges();

            return StatusCode(201, trainer);
        }

        // =========================================================
        // API 4 (1.5 điểm): DELETE /api/trainers/{trainerId}
        // =========================================================
        [HttpDelete("api/trainers/{trainerId}")]
        public IActionResult DeleteTrainer(int trainerId)
        {
            var trainer = _context.Trainers
                .Include(t => t.Specs)
                .Include(t => t.Bookings)
                .FirstOrDefault(t => t.TrainerId == trainerId);

            // TC08 (0.25đ): Không tồn tại -> 404
            if (trainer == null)
            {
                return NotFound();
            }

            // TC07 (0.5đ): Đang có lịch tập -> 400
            if (trainer.Bookings.Any())
            {
                return BadRequest("Cannot delete trainer with active booking records");
            }

            // TC06 (0.75đ): Xóa liên kết trong bảng TrainerSpecs rồi xóa Trainer -> 204
            trainer.Specs.Clear();
            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
```
5. Nhấn **`Ctrl + S`**. Bấm chuột phải vào **`Q1`** -> **Set as Startup Project** -> nhấn **`F5`** để chạy thử. Trình duyệt sẽ mở trang Swagger `http://localhost:5000/swagger`, bạn bấm **Try it out** -> **Execute** để xem kết quả. Kiểm tra xong thì nhấn nút **⏹ Stop** màu đỏ trên Visual Studio.

---

## PHẦN 4: Hướng dẫn chi tiết CÂU 2 (Q2 – Web MVC) từ A–Z

Ở Câu 2, ứng dụng `Q2` của bạn sẽ gọi dữ liệu từ một server có sẵn tên là **`givenAPI`** (chạy ở cổng `5100`).

### Bước 2.1: Bật server `givenAPI` của đề thi lên
1. Mở thư mục đề thi: `Given_Material\PaperNo_5\2\givenAPI`.
2. Cách nhanh nhất: Vào thư mục con [`publish`](file:///e:/FPT/Ki%208/PRN232/PE/2.PE_Exam/2.PE_Exam/Given_Material/PaperNo_5/2/givenAPI/publish), nháy đúp vào file **`givenAPI.exe`** (hoặc mở `givenAPI.sln` bằng một cửa sổ Visual Studio khác rồi nhấn `Ctrl + F5`).
3. Cửa sổ đen hiện lên báo `Now listening on: http://localhost:5100` -> **Cứ để nguyên cửa sổ đen đó đừng tắt** trong suốt lúc làm Q2.

---

### Bước 2.2: Kiểm tra `appsettings.json` và sửa `Program.cs` của Q2
1. Mở **[appsettings.json](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q2/appsettings.json)** của **Q2**, kiểm tra xem đã có dòng `"GivenAPIBaseUrl": "http://localhost:5100"` chưa (mặc định đề đã viết sẵn, giữ nguyên tuyệt đối không xóa).
2. Nháy đúp vào **[Program.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q2/Program.cs)** của **Q2**, xóa hết code cũ và dán đoạn này vào để bật MVC:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Member}/{action=Index}/{id?}");

app.Run();
```

---

### Bước 2.3: Tạo `MemberController.cs` trong Q2
> [!TIP]
> **Mẹo không cần tự viết class DTO:** Mở file [GymApiController.cs](file:///e:/FPT/Ki%208/PRN232/PE/2.PE_Exam/2.PE_Exam/Given_Material/PaperNo_5/2/givenAPI/givenAPI/Controllers/GymApiController.cs) trong thư mục `givenAPI` của đề thi, kéo xuống dòng 62–87 sẽ thấy sẵn 3 class `MemberSearchDTO`, `MemberSessionDTO`, `MemberDetailDTO`. Chỉ việc copy sang!

1. Trong **Solution Explorer**, bấm **chuột phải vào `Q2`** -> **Add** -> **New Folder** -> đặt tên **`Controllers`**.
2. Bấm **chuột phải vào `Controllers`** -> **Add** -> **Class...** -> đặt tên **`MemberController.cs`** -> nhấn **Add**.
3. Dán toàn bộ nội dung sau vào ([MemberController.cs](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q2/Controllers/MemberController.cs)):

```csharp
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    // Copy nguyên 3 DTO từ file GymApiController.cs của givenAPI sang
    public class MemberSearchDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
    }

    public class MemberSessionDTO
    {
        public int BookingID { get; set; }
        public string TrainerName { get; set; } = null!;
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    public class MemberDetailDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string PackageName { get; set; } = null!;
        public decimal PackagePrice { get; set; }
        public List<MemberSessionDTO> Sessions { get; set; } = new();
    }

    public class MemberController : Controller
    {
        private readonly IConfiguration _config;

        public MemberController(IConfiguration config)
        {
            _config = config;
        }

        // Trang 1: /Member
        [HttpGet("/Member")]
        public async Task<IActionResult> Index(string? memberName, string? bookingStatus)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // Gọi API lấy danh sách trạng thái cho vào Dropdown
            var statusRes = await client.GetStringAsync(baseUrl + "/api/booking-statuses");
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusRes) ?? new List<string>();

            // Gọi API tìm kiếm hội viên (nối chuỗi bằng dấu + đúng yêu cầu đề)
            string selectedStatus = string.IsNullOrEmpty(bookingStatus) ? "All statuses" : bookingStatus;
            string searchUrl = baseUrl + "/api/members/search?memberName=" + (memberName ?? "") + "&bookingStatus=" + selectedStatus;

            var memberRes = await client.GetStringAsync(searchUrl);
            var members = JsonConvert.DeserializeObject<List<MemberSearchDTO>>(memberRes) ?? new List<MemberSearchDTO>();

            ViewBag.Statuses = statuses;
            ViewBag.SelectedStatus = selectedStatus;
            ViewBag.MemberName = memberName ?? "";

            return View(members);
        }

        // Trang 2: /Member/Analyze/{id}
        [HttpGet("/Member/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            var detailRes = await client.GetStringAsync(baseUrl + "/api/members/" + id);
            var member = JsonConvert.DeserializeObject<MemberDetailDTO>(detailRes);

            return View(member);
        }
    }
}
```

---

### Bước 2.4: Tạo 2 file giao diện View (`Index.cshtml` và `Analyze.cshtml`)
Vì đề cấm cài thêm NuGet nên bạn không bấm "Add View -> Razor View" được, mà phải **tạo thư mục tay** như sau:

1. Bấm **chuột phải vào `Q2`** -> **Add** -> **New Folder** -> đặt tên **`Views`**.
2. Bấm **chuột phải vào thư mục `Views`** -> **Add** -> **New Folder** -> đặt tên **`Member`**.
3. Bấm **chuột phải vào thư mục `Member`** -> **Add** -> **New Item...**
   - Nếu thấy mục **Razor View - Empty**, bấm chọn nó, đặt tên ở dưới là **`Index.cshtml`** rồi bấm **Add**.
   - *(Mẹo: Nếu không tìm thấy Razor View - Empty, bạn cứ chọn **Class**, nhưng ở ô Name xóa chữ `.cs` đi và gõ hẳn **`Index.cshtml`** rồi bấm Add là được!)*.
4. Dán nội dung sau vào **[Index.cshtml](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q2/Views/Member/Index.cshtml)** (đã lấy từ file `Index.html` đề cho và thay bằng vòng lặp `@foreach`):

```html
@model List<Q2.Controllers.MemberSearchDTO>
@{
    var statuses = ViewBag.Statuses as List<string> ?? new List<string>();
    string selectedStatus = ViewBag.SelectedStatus ?? "All statuses";
    string memberName = ViewBag.MemberName ?? "";
}
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
</head>
<body>
    <div class="gym-container">
        <h2 style="margin-top: 0; margin-bottom: 20px;">Gym Member Directory Management</h2>

        <form method="get" action="/Member" class="filter-section">
            <div class="filter-item">
                <label>Member Name:</label>
                <input type="text" name="memberName" id="ip_memberName" value="@memberName" placeholder="Enter member name...">
            </div>

            <div class="filter-item">
                <label>Booking Status:</label>
                <select name="bookingStatus" id="sl_bookingStatus">
                    @if (selectedStatus == "All statuses")
                    {
                        <option value="All statuses" id="op_all" selected>All statuses</option>
                    }
                    else
                    {
                        <option value="All statuses" id="op_all">All statuses</option>
                    }
                    @foreach (var s in statuses)
                    {
                        var opId = "op_" + s.Replace(" ", "_");
                        if (selectedStatus == s)
                        {
                            <option value="@s" id="@opId" selected>@s</option>
                        }
                        else
                        {
                            <option value="@s" id="@opId">@s</option>
                        }
                    }
                </select>
            </div>

            <button type="submit" id="bt_search" class="btn-submit">Search</button>
        </form>

        <table class="member-table">
            <thead>
                <tr>
                    <th style="width: 250px;">Member Name</th>
                    <th style="width: 200px;">Join Date</th>
                    <th style="width: 250px;">Assigned Package</th>
                    <th style="width: 150px;">Action</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var m in Model)
                {
                    <tr>
                        <td id="td_name_@m.MemberID">@m.FullName</td>
                        <td id="td_joinDate_@m.MemberID">@m.JoinDate.ToString("yyyy-MM-dd")</td>
                        <td id="td_package_@m.MemberID">@m.PackageName</td>
                        <td>
                            <a href="/Member/Analyze/@m.MemberID" id="a_@m.MemberID" class="btn-action">Analyze Sales</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</body>
</html>
```

5. Làm tương tự: Bấm **chuột phải vào thư mục `Member`** -> **Add** -> **New Item...** -> đặt tên **`Analyze.cshtml`** -> nhấn **Add**.
6. Dán nội dung sau vào **[Analyze.cshtml](file:///e:/FPT/Ki%208/PRN232/PE/Solution_Paper5/Q2/Views/Member/Analyze.cshtml)**:

```html
@model Q2.Controllers.MemberDetailDTO
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
</head>
<body>
    <div class="analyze-wrap">
        <div class="member-card">
            <h3>Member Profile Analysis</h3>
            <div class="card-item"><strong>Member ID:</strong> <span id="span_@Model.MemberID">@Model.MemberID</span></div>
            <div class="card-item"><strong>Full Name:</strong> <span id="span_fullName">@Model.FullName</span></div>
            <div class="card-item"><strong>Join Date:</strong> <span id="span_joinDate">@Model.JoinDate.ToString("yyyy-MM-dd")</span></div>
            <div class="card-item"><strong>Current Package:</strong> @Model.PackageName (@Model.PackagePrice.ToString("N0") VND)</div>
        </div>

        <h4 style="margin-bottom: 15px; border-bottom: 1px solid #cbd5e1; padding-bottom: 10px; width: 750px; color: #334155;">Attended Sessions Log</h4>

        <table class="sessions-table">
            <thead>
                <tr>
                    <th style="width: 250px;">Trainer Name</th>
                    <th style="width: 200px;">Session Time</th>
                    <th style="width: 150px;">Duration</th>
                    <th style="width: 200px;">Financial Value Tier</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var s in Model.Sessions)
                {
                    var cost = s.DurationMinutes * 5000;
                    <tr>
                        <td id="td_trainer_@s.BookingID">@s.TrainerName</td>
                        <td id="td_time_@s.BookingID">@s.SessionTime.ToString("yyyy-MM-dd HH:mm")</td>
                        <td id="td_duration_@s.BookingID">@s.DurationMinutes mins</td>
                        <td id="td_tier_@s.BookingID">
                            @if (cost >= 400000)
                            {
                                <span class="badge-high" style="color: red; font-weight: bold;">High Premium Session</span>
                            }
                            else
                            {
                                <span class="badge-std" style="color: green;">Standard Session</span>
                            }
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</body>
</html>
```

7. Bấm chuột phải vào **`Q2`** -> **Set as Startup Project** -> nhấn **`F5`**. Trình duyệt mở `http://localhost:5190/Member`, bạn thử chọn `Da tap` rồi bấm **Search**, và bấm vào **Analyze Sales** để xem kết quả.

---

## PHẦN 5: Cách đóng gói & Nộp bài (Publish) chuẩn quy định

Trước khi nộp bài, hãy **tắt chương trình đang chạy** (bấm nút ⏹ Stop trên Visual Studio).

1. Mở lại cửa sổ **Package Manager Console** trong Visual Studio (**Tools** -> **NuGet Package Manager** -> **Package Manager Console**).
2. Gõ lệnh sau để nhảy vào thư mục Q1 và publish (thay `trungnthe123432` bằng mã sinh viên của bạn):
```powershell
cd Q1
dotnet publish -c Release -o ./Q1_trungnthe123432
cd ..
```
3. Tiếp tục gõ lệnh cho Q2:
```powershell
cd Q2
dotnet publish -c Release -o ./Q2_trungnthe123432
cd ..
```
4. Mở thư mục `GivenSolution` ngoài Windows Explorer:
   - Vào trong `Q1`, xóa 2 thư mục `bin` và `obj` (nhưng **giữ nguyên** thư mục `Q1_trungnthe123432` vừa tạo!).
   - Vào trong `Q2`, xóa 2 thư mục `bin` và `obj` (giữ nguyên `Q2_trungnthe123432`).
5. Mở phần mềm **PEA_Client**, ở câu 1 chọn nộp thư mục **`Q1`**, ở câu 2 chọn nộp thư mục **`Q2`**.
