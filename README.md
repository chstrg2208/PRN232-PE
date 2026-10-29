# PRN232 Practical Exam (PE) - Trọn Bộ Đề Thi, Barem & Lời Giải Mẫu 10/10

Kho lưu trữ tài liệu ôn thi môn **PRN232 (C# / .NET 8 Web API & MVC)** tại Đại học FPT. Được thiết kế theo tiêu chí **"Cầm tay chỉ việc"**, giúp bạn khi chuyển sang máy khác clone về là hiểu ngay quy chế thi, cấu trúc đề, chiến thuật lấy điểm và có sẵn lời giải mẫu 10/10 đã test chuẩn theo barem.

---

## 📂 1. Cấu trúc thư mục Repository

| Thư mục / File | Ý nghĩa |
|---|---|
| **`Solution_Paper5/`** | **Lời giải mẫu 10/10 điểm cho Đề 1 & Đề 5** (Q1: Trainer Web API; Q2: Member MVC). Đã test pass toàn bộ testcase. |
| **`Solution_Paper6/`** | **Lời giải mẫu 10/10 điểm cho Đề 2, 4 & 6** (Q1: Package Web API; Q2: Trainer MVC). Đã test pass toàn bộ testcase. |
| **`GivenSolution_PRN232/`** | **Khung bài thi gốc** được phát khi đi thi (solution rỗng, chỉ có Hello World). Dùng để tự luyện gõ từ đầu. |
| **`2.PE_Exam/`** | Toàn bộ tài nguyên gốc của kỳ thi: |
| ├── `PDF_DeThi/` | File PDF đề thi chính thức (Đề 1 đến Đề 6). |
| ├── `Questions/` | Ảnh chụp từng trang đề (1_page_1.jpg đến 6_page_13.jpg). |
| ├── `Barem/` | Barem chấm điểm chính thức của giảng viên (`Q13.docx`, `Q14.docx`, `Q23.docx`, `Q24.docx`). |
| └── `Given_Material/` | Script tạo Database (`database.sql`), source server `givenAPI` (port 5100) và 2 file HTML giao diện mẫu (`Index.html`, `Analyze.html`). |
| **`Huong_Dan_Giai_PE_A_Z.md`** | **Hướng dẫn cầm tay chỉ việc** từng thao tác bấm chuột trên Visual Studio 2022 cho người mới. |
| **`PRN232_PE_Summary.md`** | Tóm tắt format thi, mapping các đề, barem điểm số và các bẫy 0 điểm cần tránh. |

---

## 🗺️ 2. Ma trận 6 Mã Đề Thi

Tất cả 6 mã đề trong phòng thi đều dùng chung một Database **`PRN232_PE_SU26_13`** (Gym Management) và thực chất chỉ ghép từ **2 biến thể Q1** và **2 biến thể Q2**:

| Mã Đề | Câu 1 (Q1 - Web API port 5000) | Câu 2 (Q2 - Web MVC port 5190) | Lời giải mẫu tương ứng |
|:---:|---|---|:---:|
| **Đề 1 & Đề 5** | **Biến thể A:** Quản lý HLV (`Specializations`, `filter-trainers`, CRUD HLV) | **Biến thể A:** Danh sách Hội viên (`/Member`, `/Member/Analyze/{id}`) | Xem thư mục **`Solution_Paper5/`** |
| **Đề 2, 4 & 6** | **Biến thể B:** Quản lý Gói tập (`MembershipPackages`, `filter-members`, CRUD Gói) | **Biến thể B:** Danh sách HLV (`/Trainer`, `/Trainer/Analyze/{id}`) | Xem thư mục **`Solution_Paper6/`** |
| **Đề 3** | **Biến thể B:** Quản lý Gói tập (như Đề 6) | **Biến thể A:** Danh sách Hội viên (như Đề 5) | Lấy Q1 ở `Solution_Paper6` + Q2 ở `Solution_Paper5` |

👉 **Chỉ cần nắm vững 2 bài giải `Solution_Paper5` và `Solution_Paper6` là bạn bao trọn 100% đề thi.**

---

## ⚠️ 3. 3 "Bẫy Tử Thần" Bị 0 Điểm Cần Tránh Tuyệt Đối

1. ❌ **Cấm cài thêm thư viện qua NuGet Package Manager:** Bài thi bị tính gian lận / invalid nếu thêm bất kỳ package nào ngoài các package có sẵn trong khung `GivenSolution`.
   - Q1 đã có sẵn: EF Core SqlServer + Design, Swagger, OData, JwtBearer.
   - Q2 chỉ có sẵn: `Newtonsoft.Json`. Vì không có package sinh view nên **View trong Q2 phải tạo tay** (xem hướng dẫn bên dưới).
2. ❌ **Chuỗi kết nối / URL bắt buộc nằm trong `appsettings.json`:**
   - Q1: Phải nằm trong `"ConnectionStrings": { "MyCnn": "..." }`.
   - Q2: Phải nằm trong `"GivenAPIBaseUrl": "http://localhost:5100"`.
   - Nếu hardcode trong code C# -> **0 điểm câu đó**.
3. ❌ **Sai `id` thẻ HTML hoặc sai URL trong Q2:** Máy chấm tự động bằng tool (quét Selenium theo đúng `id` và `innerText`), không chấm giao diện đẹp hay xấu. Sai `id` = mất trắng điểm testcase đó.

---

## 🎯 4. Phổ Điểm & Chiến Thuật "Chỉ Cần Qua Môn" (≥ 4.0 Điểm)

Tổng điểm PE là **10.0** (Q1 = 5.0đ, Q2 = 5.0đ). Điểm qua môn thường là **≥ 4.0 điểm**.

### Cách kiếm 4.0 điểm ở Q1 (Bỏ qua phần LINQ phức tạp):
- `TC01`: Lấy danh sách kèm đếm số lượng (`GET /api/Specializations` hoặc `/api/MembershipPackages`): **1.0 điểm** (Rất dễ).
- `TC03`: Validate số âm trả về 400 (`minExperience < 0` hoặc `minDuration < 0`): **0.25 điểm** (1 câu `if`).
- `TC04` + `TC05`: Thêm mới POST (201 Created) và bắt trùng (400 Bad Request): **1.25 điểm** (Dễ).
- `TC06` + `TC07` + `TC08`: Xóa DELETE (204 No Content, bắt khóa ngoại 400, không tìm thấy 404): **1.50 điểm** (Dễ).
=> **Tổng cộng cầm chắc 4.0 / 5.0 điểm ở Q1 mà không cần đụng tới câu lọc tính tổng phút (TC02 - 1.0đ)!**

### Cách kiếm 3.0 - 5.0 điểm ở Q2:
- Copy toàn bộ HTML từ file mẫu đề cho (`Index.html`, `Analyze.html`), chỉ thay dữ liệu tĩnh bằng vòng lặp `@foreach`.
- Trang Index: Hiển thị bảng (**1.0đ**) + Dropdown filter (**1.0đ**) + Nút Link Analyze (**0.5đ**) + Chức năng Search (**0.5đ**) = **3.0 điểm**.
- Trang Analyze: Hiển thị thông tin cơ bản (**1.0đ**) + Bảng lịch hẹn (**1.0đ**) = **2.0 điểm**.

---

## 🚀 5. Hướng Dẫn Nhanh Khi Sang Máy Mới (Quickstart)

### Bước 1: Chạy Script Tạo Database
Mở **SQL Server Management Studio (SSMS)**:
- Kết nối tới server của máy (thường là `.` hoặc `(localdb)\MSSQLLocalDB` hoặc `localhost`).
- Mở file `2.PE_Exam/2.PE_Exam/Given_Material/PaperNo_5/1/database.sql`.
- Nhấn **F5** (Execute) để tạo database `PRN232_PE_SU26_13`.

### Bước 2: Cài Đặt Công Cụ EF Core CLI (Nếu máy chưa có)
Mở PowerShell hoặc CMD gõ:
```powershell
dotnet tool install --global dotnet-ef
```
*(Nếu máy đã cài sẵn thì bỏ qua bước này).*

### Bước 3: Chạy và Test Thử Bài Giải Mẫu

#### Chạy Q1 (Web API - port 5000):
1. Mở `Solution_Paper5/GivenSolution.sln` bằng Visual Studio 2022.
2. Kiểm tra chuỗi kết nối trong `Q1/appsettings.json` cho khớp với SQL máy bạn.
3. Chuột phải vào project **Q1** -> **Set as Startup Project** -> bấm **F5**.
4. Trình duyệt tự mở Swagger tại: `http://localhost:5000/swagger`.

#### Chạy Q2 (Web MVC - port 5190):
1. Trước khi chạy Q2, **phải bật server `givenAPI`** (nằm ở `2.PE_Exam/2.PE_Exam/Given_Material/PaperNo_5/2/givenAPI/publish/givenAPI.exe`) để lắng nghe tại `http://localhost:5100`.
2. Trên Visual Studio, chuột phải vào project **Q2** -> **Set as Startup Project** -> bấm **F5**.
3. Trình duyệt mở giao diện tại: `http://localhost:5190/Member`.

---

## 🛠️ 6. Các Lệnh Quan Trọng Cần Nhớ Trong Phòng Thi

### Lệnh Scaffold Entity Framework Core (Tạo Models từ Database):
Mở **Tools** -> **NuGet Package Manager** -> **Package Manager Console** (chọn Default project là **`Q1`**):
```powershell
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=PRN232_PE_SU26_13;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models --no-onconfiguring
```

### Lệnh Đóng Gói (Publish) Để Nộp Bài:
Mở Package Manager Console hoặc Terminal tại thư mục chứa bài làm:
```powershell
# Publish Q1
cd Q1
dotnet publish -c Release -o ./Q1_<MãSinhViên>
cd ..

# Publish Q2
cd Q2
dotnet publish -c Release -o ./Q2_<MãSinhViên>
cd ..
```
*Lưu ý: Sau khi publish xong, vào thư mục Q1 và Q2 xóa sạch thư mục `bin/` và `obj/` (giữ lại thư mục `Q1_<MãSinhViên>`) trước khi nộp vào phần mềm PEA_Client.*

---

## 📖 7. Đọc Hướng Dẫn Chi Tiết
Để xem hướng dẫn từng bước click chuột, code mẫu có chú thích tiếng Việt từng dòng, vui lòng đọc tiếp file:
👉 **[Huong_Dan_Giai_PE_A_Z.md](./Huong_Dan_Giai_PE_A_Z.md)**
