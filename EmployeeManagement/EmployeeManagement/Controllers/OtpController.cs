using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public OtpController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] LoginRequest req)
        {
            var isRegistered = await _context.Users.AnyAsync(u => u.UserEmail == req.Email);
            if (!isRegistered)
            {
                return Unauthorized("This email is not registered. Please register first.");
            }

            var timeWindowStart = DateTime.UtcNow.AddMinutes(-15);
            var recentOtpCount = await _context.OtpLogins
                .CountAsync(o => o.Email == req.Email && o.GeneratedAt >= timeWindowStart);

            if (recentOtpCount >= 5)
            {
                return BadRequest("OTP generation limit reached. Try again after 15 minutes.");
            }

            var oneAndHalfMinuteAgo = DateTime.UtcNow.AddSeconds(-90);

            var existingOtp = await _context.OtpLogins
                .Where(o => o.Email == req.Email && o.GeneratedAt >= oneAndHalfMinuteAgo)
                .OrderByDescending(o => o.GeneratedAt)
                .FirstOrDefaultAsync();

            if (existingOtp != null)
            {
                var timeElapsed = DateTime.UtcNow - existingOtp.GeneratedAt;
                var remainingSeconds = 90 - (int)timeElapsed.TotalSeconds;

                if (remainingSeconds > 0)
                {
                    return Ok(new
                    {
                        message = $"Existing OTP is still valid please try after {remainingSeconds} seconds."
                    });
                }
            }

            // ✅ Always remove all existing OTPs for the email (clean state)
            var oldOtps = await _context.OtpLogins
                .Where(o => o.Email == req.Email)
                .ToListAsync();

            if (oldOtps.Any())
            {
                _context.OtpLogins.RemoveRange(oldOtps);
                await _context.SaveChangesAsync();
            }

            int otp = new Random().Next(100000, 999999);

            var entry = new OtpLogin
            {
                Email = req.Email,
                OtpCode = otp,
                GeneratedAt = DateTime.UtcNow
            };

            _context.OtpLogins.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(new { otp, message = "New OTP generated." });
        }


        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);

            var validOtp = await _context.OtpLogins
                .Where(o => o.Email == req.Email && o.OtpCode == req.Otp && o.GeneratedAt >= oneMinuteAgo)
                .OrderByDescending(o => o.GeneratedAt)
                .FirstOrDefaultAsync();

            if (validOtp == null)
            {
                return Unauthorized("OTP is invalid or expired.");
            }

            return Ok(new { success = true });
        }
        public record LoginRequest(
            [Required]
            [EmailAddress]
            string Email
        );

        public record VerifyRequest(
            [Required]
            [EmailAddress]
            string Email,
            [Required]
            [Range(100000, 999999)]
            int Otp
        );
    }
}
