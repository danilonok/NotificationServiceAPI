using NotificationServiceAPI.Data;
using Automatonymous;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationServiceAPI.Models;
using System.Security.Claims;

namespace NotificationServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<NotificationSettingsController> _logger;
        public  NotificationSettingsController(IConfiguration configuration, AppDBContext db_context, ILogger<NotificationSettingsController> logger)
        {
            _context = db_context;
            _logger = logger;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SetNotificationSettings([FromBody] UserNotificationSettings settings)
        {
            if (settings != null)
            {
                UserNotificationSettings new_settings = new UserNotificationSettings()
                {
                    NotifyBeforeHours = settings.NotifyBeforeHours,
                    UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    Email = settings.Email,
                    IsNotificationEnabled = settings.IsNotificationEnabled,
                };

                _context.NotificationSettings.Add(new_settings);
                await _context.SaveChangesAsync();
                return Ok(new { id = new_settings.UserId, message = "Settings updated successfully." });
            }
            return BadRequest("Event parameters are invalid.");

        }
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetEvent()
        {

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            UserNotificationSettings sett = await _context.NotificationSettings.FirstOrDefaultAsync(u => u.UserId == userId);
            if (sett == null)
            {
                return NotFound("Settings not found or you don't have permission to access this settings.");
            }
            return Ok(sett);
        }

        
    }
}
