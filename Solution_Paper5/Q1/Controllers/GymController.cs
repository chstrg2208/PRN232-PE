using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    // Du lieu nhan vao khi POST /api/trainers
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

        // 1. GET /api/Specializations
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

        // 2. GET /api/filter-trainers?specId=1&minExperience=2
        [HttpGet("api/filter-trainers")]
        public IActionResult FilterTrainers(int? specId, int? minExperience)
        {
            if (minExperience.HasValue && minExperience.Value < 0)
            {
                return BadRequest("minExperience cannot be negative");
            }

            // Lay het du lieu lien quan len bo nho roi xu ly bang LINQ cho de
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
                    // Tat ca chi tiet buoi tap cua HLV, buoi moi nhat dung dau
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

        // 3. POST /api/trainers
        [HttpPost("api/trainers")]
        public IActionResult CreateTrainer([FromBody] TrainerCreateDto dto)
        {
            string email = (dto.Email ?? "").ToLower();

            bool exists = _context.Trainers.Any(t => t.Email != null && t.Email.ToLower() == email);
            if (exists)
            {
                return BadRequest("Trainer email already exists");
            }

            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,
                Email = dto.Email,
                ExperienceYears = dto.ExperienceYears
            };

            _context.Trainers.Add(trainer);
            _context.SaveChanges();

            // Bookings va Specs da duoc khoi tao = new List<>() nen se tra ve []
            return StatusCode(201, trainer);
        }

        // 4. DELETE /api/trainers/{trainerId}
        [HttpDelete("api/trainers/{trainerId}")]
        public IActionResult DeleteTrainer(int trainerId)
        {
            var trainer = _context.Trainers
                .Include(t => t.Specs)
                .Include(t => t.Bookings)
                .FirstOrDefault(t => t.TrainerId == trainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            if (trainer.Bookings.Any())
            {
                return BadRequest("Cannot delete trainer with active booking records");
            }

            trainer.Specs.Clear();          // xoa cac dong trong bang TrainerSpecs
            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
