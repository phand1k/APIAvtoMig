using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIAvtoMig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private ApplicationDbContext context;
        public AuthenticateController(
            UserManager<AspNetUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, ApplicationDbContext _context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            context = _context;
        }
        [HttpPost]
        [Route("resendsms")]
        public async Task<IActionResult> ResendSms([FromBody] ResendSms model)
        {
            var userExists = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (userExists == null)
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found!" });

            SmsActivate smsActivate = new SmsActivate();
            smsActivate.PhoneNumber = model.PhoneNumber;
            smsActivate.Code = RandomModel.GetRandomNumber();
            smsActivate.DateOfEndSMS = DateTime.Now.AddMinutes(10);

            await context.SmsActivates.AddAsync(smsActivate);
            await context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Sms has been resended!" });
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            bool accountConfirmed = user.PhoneNumberConfirmed;
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (accountConfirmed)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.PhoneNumber),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims, user.Id, user.OrganizationId);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }//
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "Phone number is not confirmed!" });
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("confirmphonenumber")]
        public async Task<IActionResult> ConfirmPhoneNumber([FromBody] ConfirmPhoneNumberModel model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user != null)
            {
                if (user.PhoneNumberConfirmed)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = "Phone number is already confirmed!" });
                }
                var smsActivate = await context.SmsActivates.
                    Where(x=>x.IsUsed == false && x.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync(x=>x.Code == model.Code);
                if (smsActivate != null)
                {
                    smsActivate.IsUsed = true;
                    user.PhoneNumberConfirmed = true;
                    await context.SaveChangesAsync();
                    return Ok(new Response { Status = "Success", Message = "Phone number confirmed successfully!" });
                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "400 bad request", Message = "Incorrect code or code has expired!" });
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response {Status = "404", Message = "User not found" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            AspNetUser user = new()
            {
                Email = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber
            };
            var checkOrganizationExists = await context.Organizations.FirstOrDefaultAsync(x=>x.Number == model.OrganizationId);
            if (checkOrganizationExists != null)
            {
                user.OrganizationId = checkOrganizationExists.Id;
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                SmsActivate smsActivate = new SmsActivate();
                smsActivate.PhoneNumber = model.PhoneNumber;
                smsActivate.Code = RandomModel.GetRandomNumber();
                smsActivate.DateOfEndSMS = DateTime.Now.AddMinutes(10);

                await context.SmsActivates.AddAsync(smsActivate);
                await context.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Organization does not exists!" });

        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            var user = new AspNetUser
            {
                Email = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims, string userId, int? organizationId)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            authClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

            // Преобразуйте organizationId в строку перед добавлением в Claim
            if (organizationId.HasValue)
            {
                authClaims.Add(new Claim("OrganizationId", organizationId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(2190),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

    }
}
