using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    // --- Copy 3 class DTO tu GivenAPIs/Controllers/GymApiController.cs ---
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

        // URL: /Member
        [HttpGet("/Member")]
        public async Task<IActionResult> Index(string? memberName, string? bookingStatus)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // 1. Lay danh sach status cho dropdown
            var statusRes = await client.GetStringAsync(baseUrl + "/api/booking-statuses");
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusRes) ?? new List<string>();

            // 2. Goi API tim kiem hoi vien (noi chuoi tuong minh theo yeu cau de)
            string selectedStatus = string.IsNullOrEmpty(bookingStatus) ? "All statuses" : bookingStatus;
            string searchUrl = baseUrl + "/api/members/search?memberName=" + (memberName ?? "") + "&bookingStatus=" + selectedStatus;

            var memberRes = await client.GetStringAsync(searchUrl);
            var members = JsonConvert.DeserializeObject<List<MemberSearchDTO>>(memberRes) ?? new List<MemberSearchDTO>();

            ViewBag.Statuses = statuses;
            ViewBag.SelectedStatus = selectedStatus;
            ViewBag.MemberName = memberName ?? "";

            return View(members);
        }

        // URL: /Member/Analyze/{id}
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
