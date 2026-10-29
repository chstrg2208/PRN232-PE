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

        // 1. GET /api/MembershipPackages
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

        // 2. GET /api/filter-members?packageId=1&minDuration=50
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
                .ToList();

            if (packageId.HasValue)
            {
                members = members.Where(m => m.PackageId == packageId.Value).ToList();
            }

            var result = new List<object>();

            foreach (var m in members)
            {
                var details = m.BookingDetails
                    .OrderByDescending(bd => bd.Booking?.SessionTime)
                    .ToList();

                int totalMinutes = details.Sum(bd => bd.DurationMinutes ?? 0);

                if (minDuration.HasValue && totalMinutes < minDuration.Value)
                {
                    continue;
                }

                var lastBooking = details.FirstOrDefault()?.Booking;
                string lastTrainer = lastBooking?.Trainer?.TrainerName ?? "No Bookings";

                // Format DateTime as "yyyy-MM-ddTHH:mm:ss"
                string joinDateStr = m.JoinDate.HasValue 
                    ? m.JoinDate.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-ddTHH:mm:ss")
                    : "";

                result.Add(new
                {
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

        // 3. POST /api/MembershipPackages
        [HttpPost("api/MembershipPackages")]
        public IActionResult CreatePackage([FromBody] PackageCreateDto dto)
        {
            string pkgName = (dto.PackageName ?? "").Trim();

            bool exists = _context.MembershipPackages
                .Any(p => p.PackageName != null && p.PackageName.ToLower() == pkgName.ToLower());

            if (exists)
            {
                return BadRequest("Membership package name already exists");
            }

            var pkg = new MembershipPackage
            {
                PackageName = dto.PackageName,
                Price = dto.Price
            };

            _context.MembershipPackages.Add(pkg);
            _context.SaveChanges();

            return StatusCode(201, pkg);
        }

        // 4. DELETE /api/MembershipPackages/{packageId}
        [HttpDelete("api/MembershipPackages/{packageId}")]
        public IActionResult DeletePackage(int packageId)
        {
            var pkg = _context.MembershipPackages
                .Include(p => p.Members)
                .FirstOrDefault(p => p.PackageId == packageId);

            if (pkg == null)
            {
                return NotFound();
            }

            if (pkg.Members.Any())
            {
                return BadRequest("Cannot delete membership package with active registered members");
            }

            _context.MembershipPackages.Remove(pkg);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
