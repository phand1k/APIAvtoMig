using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly ApplicationDbContext _context;

    public OrganizationController(UserManager<AspNetUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] Organization organization)
    {
        var organizationExists = await _context.Organizations
            .Where(x => x.Number == organization.Number)
            .FirstOrDefaultAsync();

        if (organizationExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Organization already exists!" });

        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
        AspNetUser defaultUser = new()
        {
            Email = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            PhoneNumber = Guid.NewGuid().ToString(),
            FirstName = "Default",
            LastName = "User",
            PasswordHash = Guid.NewGuid().ToString(),
            NormalizedEmail = Guid.NewGuid().ToString(),
        };
        defaultUser.OrganizationId = organization.Id;

        await _context.AspNetUsers.AddAsync(defaultUser);

        var subscription = new Subscription();
        subscription.OrganizationId = organization.Id;
        subscription.DateOfEndSubscription = DateTime.Now.AddDays(1);
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        return Ok(new Response { Status = "Success", Message = "Organization created successfully!" });
    }
    [HttpPost]
    [Route("SendPushNotification")]
    public async Task<IActionResult> SendPushNotification([FromBody] PushNotificationRequest request)
    {
        try
        {
            var pushNotificationService = new PushNotificationService();
            await pushNotificationService.SendPushNotification(request.DeviceToken, request.Title, request.Body);

            return Ok(new { Success = true, Message = "Push notification sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }
    [HttpGet]
    [Route("ListOfTypeOfOrganizations")]
    public async Task<IActionResult> ListOfTypeOfOrganizations()
    {
        var list = await _context.TypeOfOrganizations
    .Select(x => new {
        x.Id,
        x.Name
    })
    .ToListAsync();
        return Ok(list);
    }
    [Authorize]
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> List()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);


        var userId = await _context.AspNetUsers.
            Where(x=>x.UserName == userName).Select(x=>x.Id).FirstOrDefaultAsync();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized();
        }

        var organizations = await _context.Organizations
            .Where(o => o.Id == user.OrganizationId)
            .ToListAsync();

        return Ok(organizations);
    }
}
