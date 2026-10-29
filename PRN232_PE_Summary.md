# PRN232 PE – Tóm tắt đề ôn & format thi

## 1. Format đi thi (GivenSolution)

Solution [GivenSolution.sln](file:///e:/FPT/Ki%208/PRN232/PE/GivenSolution_PRN232/GivenSolution/GivenSolution.sln) gồm 2 project rỗng, **.NET 8**, `Program.cs` chỉ có `MapGet("/", "Hello World!")`:

| Project | Port | NuGet có sẵn (KHÔNG được thêm) | appsettings |
|---|---|---|---|
| **Q1** (Web API) | `http://localhost:5000`, launchUrl `swagger` | JwtBearer 8.0.17, **OData 9.3.2**, EF Core **Design** + **SqlServer** 8.0.17, Swashbuckle 9.0.1 | `"ConnectionStrings": { "MyCnn": "" }` |
| **Q2** (MVC/Razor) | `http://localhost:5190` | chỉ **Newtonsoft.Json** 13.0.3 | `"GivenAPIBaseUrl": "http://localhost:5100"` |

> [!IMPORTANT]
> - Phải dùng đúng solution được phát, không thêm thư viện NuGet nào → Q2 **không có code generator**, view phải tự viết tay. Q1 scaffold DB bằng `dotnet ef dbcontext scaffold` (cần có global tool `dotnet-ef`, package Design đã có sẵn).
> - Q1 phải tự thêm `AddControllers()`, Swagger, `AddDbContext` (đọc `MyCnn`) vào `Program.cs`. Q2 phải tự thêm `AddControllersWithViews()` / `AddRazorPages()` + route.
> - Nộp bài: `dotnet publish -c Release -o ./Q1_<account>` cho từng câu, nộp thư mục gốc project (được xoá `bin/`, `obj/`).
> - Vi phạm 1 yêu cầu → bài bị tính **không hợp lệ**. Connection string / BaseUrl không nằm trong appsettings.json → **câu đó 0 điểm**.

## 2. Cấu trúc bộ đề (`2.PE_Exam`)

- `PDF_DeThi/` + `Questions/1..6/` (ảnh từng trang) — 6 mã đề.
- `Given_Material/PaperNo_5`, `PaperNo_6` — `database.sql` + `givenAPI` (in‑memory) + 2 file HTML mẫu (Index/Analyze).
- `Barem/` — barem chấm: **Q13, Q14** (Q1 – API), **Q23, Q24** (Q2 – MVC).

Cùng một DB **Gym** (`PRN232_PE_SU26_13`): `MembershipPackages`, `Members`, `Trainers`, `Specializations`, `Bookings`, `BookingDetails` (PK kép, n‑n Booking–Member), `TrainerSpecs` (n‑n Trainer–Spec thuần, EF scaffold ra skip‑navigation `Trainer.Specs` / `Specialization.Trainers`).

Đề được ghép từ 2 biến thể Q1 × 2 biến thể Q2:

| Đề | Q1 (Web API + EF Core) | Q2 (MVC gọi Given API) |
|---|---|---|
| 1, 5 | **A – Trainer** (barem Q13) | **A – Member** (barem Q24) |
| 2, 4, 6 | **B – Package** (barem Q14) | **B – Trainer** (barem Q23) |
| 3 | **B – Package** | **A – Member** |

## 3. Q1 – Web API (5 điểm, 8 testcase, DB reset trước mỗi TC)

Chấm **Status code + Content‑Type + Body**. Lỗi 400 phải là **`text/plain; charset=utf-8`** → dùng `return BadRequest("message");` (string). JSON là `application/json; charset=utf-8`, camelCase.

### Biến thể A – Trainer

| TC | Endpoint | Yêu cầu | Điểm |
|---|---|---|---|
| 01 | `GET /api/Specializations` | `specId, specName, description, trainerCount` (đếm TrainerSpecs) | 1.0 |
| 02 | `GET /api/filter-trainers?specId=&minExperience=` | lọc theo spec + `ExperienceYears >= min`; trả `trainerId, trainerName, email, experienceYears, totalMinutesTrained` (Σ DurationMinutes, 0 nếu không có), `lastBookedMember` (FullName theo `SessionTime` desc, không có → `"No Bookings"`), `specializationList` (list SpecName); **sort ExperienceYears desc** | 1.0 |
| 03 | như trên, `minExperience=-2` | 400 `minExperience cannot be negative` | 0.25 |
| 04 | `POST /api/trainers` `{trainerName,email,experienceYears}` | **201**, trả entity Trainer với `bookings: []`, `specs: []` (không null), id mong đợi = 7 | 0.75 |
| 05 | POST email trùng (không phân biệt hoa thường) | 400 `Trainer email already exists` | 0.5 |
| 06 | `DELETE /api/trainers/6` | xoá luôn dòng TrainerSpecs → **204** | 0.75 |
| 07 | `DELETE /api/trainers/1` (còn Bookings) | 400 `Cannot delete trainer with active booking records` | 0.5 |
| 08 | `DELETE /api/trainers/99` | 404 | 0.25 |

Ví dụ TC02 (`specId=1&minExperience=2`): trainer 1 (HLV Nguyen Hoan, 60 phút, Cao Van Nam, [Giam can nhanh, Tang co bap]) và trainer 4 (HLV Pham Huy, 45, Phan Thanh Thao, [Giam can nhanh]).

### Biến thể B – Package

| TC | Endpoint | Yêu cầu | Điểm |
|---|---|---|---|
| 01 | `GET /api/MembershipPackages` | `packageId, packageName, price, memberCount` (6 gói) | 1.0 |
| 02 | `GET /api/filter-members?packageId=&minDuration=` | ví dụ `packageId=1&minDuration=50` → `memberId, fullName, joinDate ("2026-01-10T00:00:00"), packageName, totalMinutesTrained, lastTrainerName` | 1.0 |
| 03 | `minDuration=-50` | 400 `minDuration cannot be negative` | 0.25 |
| 04 | `POST /api/MembershipPackages` `{packageName, price}` | 201, `packageId: 7`, `members: []` | 0.75 |
| 05 | tên gói trùng | 400 `Membership package name already exists` | 0.5 |
| 06 | `DELETE /api/MembershipPackages/6` | 204 | 0.75 |
| 07 | `DELETE .../1` (còn member) | 400 `Cannot delete membership package with active registered members` | 0.5 |
| 08 | `DELETE .../99` | 404 | 0.25 |

> [!WARNING]
> - Cần `ReferenceHandler.IgnoreCycles` khi trả entity có navigation (TC04).
> - Chú ý dữ liệu DB ≠ dữ liệu givenAPI: trong DB trainer 1 là **HLV Nguyen Hoan** và có 6 trainer / 6 package; givenAPI (Q2) dùng **HLV Nguyen Manh**, 5 trainer / 5 package.

## 4. Q2 – MVC/Razor gọi Given API (5 điểm, 6 testcase)

Bắt buộc: dùng **HttpClient**, `GivenAPIBaseUrl` trong appsettings.json, **nối chuỗi tường minh** (`baseUrl + "/api/..."`), mọi phần tử input/output phải có **`id`** đúng chuẩn (chấm tự động theo id + innerText; không chấm giao diện). Chạy givenAPI ở port 5100.

### Biến thể A – Member (Paper 1, 3, 5)

API cho sẵn: `GET /api/booking-statuses`, `GET /api/members/search?memberName=&bookingStatus=`, `GET /api/members/{id}`.

| TC | Trang | ID / kết quả | Điểm |
|---|---|---|---|
| 01 | `/Member` | `td_name_{id}`, `td_joinDate_{id}` (yyyy-MM-dd), `td_package_{id}` – 5 member | 1.0 |
| 02 | `/Member` | `sl_bookingStatus`: `op_all` "All statuses" (**selected mặc định**), `op_Da_tap`, `op_Cho_mien`, `op_Da_huy` (khoảng trắng → `_`); `ip_memberName`, `bt_search` | 1.0 |
| 03 | `/Member` | `a_{id}` text **"Analyze Sales"**, href `/Member/Analyze/{id}` | 0.5 |
| 04 | `/Member/Analyze/1` | `span_1`, `span_fullName`, `span_joinDate` | 1.0 |
| 05 | `/Member/Analyze/1` | `td_trainer_{BookingID}`, `td_time_{BookingID}` (yyyy-MM-dd HH:mm), `td_duration_{BookingID}` ("60 mins"), `td_tier_{BookingID}`: cost = phút × 5000; ≥ 400.000 → **"High Premium Session"** (đỏ, đậm), ngược lại **"Standard Session"** (xanh) | 1.0 |
| 06 | `/Member` | chọn option index 1 (Da tap) + Search → member 1, 2, 5 | 0.5 |

Analyze còn hiển thị "Current Package: Goi Thang Co Ban (300,000 VND)".

### Biến thể B – Trainer (Paper 2, 4, 6)

| TC | Trang | ID / kết quả | Điểm |
|---|---|---|---|
| 01 | `/Trainer` | `td_name_{id}`, `td_email_{id}`, `td_exp_{id}`, badge `span_{SpecName}_{SpecId}` (giữ khoảng trắng, vd `span_Giam can nhanh_1`) | 1.0 |
| 02 | `/Trainer` | `sl_specializations`: `op_0` "All specializations" (selected, value 0), `op_{specId}`; `ip_trainerName`, `bt_search` | 1.0 |
| 03 | `/Trainer` | `a_{id}` text **"Analyze Schedule"** → `/Trainer/Analyze/{id}` | 0.5 |
| 04 | `/Trainer/Analyze/1` | `span_1`, `span_trainerName`, `span_experienceYears` | 1.0 |
| 05 | `/Trainer/Analyze/1` | `td_member_{MemberID}`, `td_time_{BookingID}`, `td_duration_{BookingID}_{MemberID}`, `td_intensity_{BookingID}_{MemberID}`: phút > 60 → **"High Intensity"** (đỏ, đậm), ngược lại **"Standard"** (xanh) | 1.0 |
| 06 | `/Trainer` | chọn spec 1 + Search → chỉ trainer 1 và 4 | 0.5 |
