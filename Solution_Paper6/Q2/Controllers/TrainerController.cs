using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Q2.Controllers
{
    public class SpecializationDTO
    {
        public int SpecID { get; set; }
        public string SpecName { get; set; } = null!;
    }

    public class TrainerSearchDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
    }

    public class BookingSessionDTO
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; } = null!;
        public int BookingID { get; set; }
        public DateTime SessionTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = null!;
    }

    public class TrainerDetailDTO
    {
        public int TrainerID { get; set; }
        public string TrainerName { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public List<SpecializationDTO> Specializations { get; set; } = new();
        public List<BookingSessionDTO> BookingSessions { get; set; } = new();
    }

    public class TrainerController : Controller
    {
        private readonly IConfiguration _config;

        public TrainerController(IConfiguration config)
        {
            _config = config;
        }

        // URL: /Trainer
        [HttpGet("/Trainer")]
        public async Task<IActionResult> Index(int specId = 0, string? trainerName = null)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            // 1. Lay danh sach chuyen mon do vao dropdown
            var specRes = await client.GetStringAsync(baseUrl + "/api/specializations");
            var specs = JsonConvert.DeserializeObject<List<SpecializationDTO>>(specRes) ?? new List<SpecializationDTO>();

            // 2. Goi API search trainers (noi chuoi tuong minh)
            string searchUrl = baseUrl + "/api/trainers/search?specId=" + specId + "&trainerName=" + (trainerName ?? "");
            var trainerRes = await client.GetStringAsync(searchUrl);
            var trainers = JsonConvert.DeserializeObject<List<TrainerSearchDTO>>(trainerRes) ?? new List<TrainerSearchDTO>();

            ViewBag.Specs = specs;
            ViewBag.SelectedSpecId = specId;
            ViewBag.TrainerName = trainerName ?? "";

            return View(trainers);
        }

        // URL: /Trainer/Analyze/{id}
        [HttpGet("/Trainer/Analyze/{id}")]
        public async Task<IActionResult> Analyze(int id)
        {
            string baseUrl = _config["GivenAPIBaseUrl"]!;
            using HttpClient client = new HttpClient();

            var detailRes = await client.GetStringAsync(baseUrl + "/api/trainers/" + id);
            var trainer = JsonConvert.DeserializeObject<TrainerDetailDTO>(detailRes);

            return View(trainer);
        }
    }
}
