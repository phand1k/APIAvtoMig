using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIAvtoMig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        public ServiceController(ApplicationDbContext context, UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost]
        [Route("CreateService")]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            var serviceExists = await _context.Services
            .Where(x => x.Name == service.Name && x.IsDeleted == false)
            .FirstOrDefaultAsync();
            if (serviceExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Service already exists!" });


            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _context.AspNetUsers.
                Where(x => x.UserName == userName).Select(x => x.Id).FirstOrDefaultAsync();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            int? organizationId = await _context.AspNetUsers.
                Where(x => x.Id == userId).Select(x => x.OrganizationId).FirstOrDefaultAsync();

            service.AspNetUserId = userId;
            service.OrganizationId = organizationId;
            if (service.AspNetUserId == null)
            {
                return NotFound();
            }

            if (service.OrganizationId == null)
            {
                return Unauthorized();
            }
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Service created successfully!" });
        }
        [Authorize]
        [HttpGet]
        [Route("Services")]
        public async Task<IActionResult> Services()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);

            var userId = await _context.AspNetUsers
                .Where(x => x.UserName == userName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }
            var services = await _context.Services.Where(x => x.IsDeleted == false && x.OrganizationId == user.OrganizationId).
                Select(x => new {
                    x.Id,
                    x.Name,
                    x.Price
                }).ToListAsync();

            if (services == null)
            {
                return BadRequest();
            }

            return Ok(services);
        }
        [Authorize]
        [Route("DeleteService")]
        [HttpPatch]
        public async Task<IActionResult> DeleteService(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Service not found!" });
            }
            var service = await _context.Services.
                Where(x=>x.IsDeleted == false).FirstOrDefaultAsync(x=>x.Id == id);
            if (service == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Service not found!" });
            }
            service.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize]
        [Route("EditService")]
        [HttpPatch]
        public async Task<IActionResult> EditService(int? id, [FromBody] Service? service)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Service not found!" });
            }
            var existingService = await _context.Services.
                Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Service not found!" });
            }
            existingService.Name = service.Name;
            existingService.Price = service.Price;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
