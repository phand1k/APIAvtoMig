﻿using APIAvtoMig.Auth;
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
    public class OrderController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        public OrderController(ApplicationDbContext context, UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] WashOrder order)
        {
            var orderExists = await _context.WashOrders
            .Where(x => x.CarNumber == order.CarNumber && x.IsDeleted == false).Where(x=>x.IsOvered == false)
            .FirstOrDefaultAsync();
            if (orderExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Order already exists!" });


            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _context.AspNetUsers.
                Where(x => x.UserName == userName).Select(x => x.Id).FirstOrDefaultAsync();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            int? organizationId = await _context.AspNetUsers.
                Where(x=>x.Id == userId).Select(x=>x.OrganizationId).FirstOrDefaultAsync();

            order.AspNetUserId = userId;
            order.OrganizationId = organizationId;
            if (order.AspNetUserId == null)
            {
                return NotFound();
            }
            if (order.OrganizationId == null)
            {
                return Unauthorized();
            }
            await _context.WashOrders.AddAsync(order);
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Order created successfully!" });
        }
        
        [Authorize]
        [HttpGet]
        [Route("WashOrders")]
        public async Task<IActionResult> WashOrders()
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
            var washOrders = await _context.WashOrders
    .Include(x => x.ModelCar.Car).Where(x=>x.IsDeleted == false)
    .Where(x => x.AspNetUserId == user.Id && x.OrganizationId == user.OrganizationId)
    .Select(x => new {
        x.Id,
        x.CarNumber,
        Car = new
        {
            x.Car.Id,
            x.Car.Name
        },
        ModelCar = new
        {
            x.ModelCar.Id,
            x.ModelCar.Name
        }
    })
    .ToListAsync();

            if (washOrders == null)
            {
                return BadRequest();
            }

            return Ok(washOrders);
        }


        [Authorize]
        [HttpGet]
        [Route("ListOf")]
        public async Task<IActionResult> ListOf()
        {
            var list = await _context.WashOrders.ToListAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet]
        [Route("ListOfUsers")]
        public async Task<IActionResult> ListOfUsers()
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
            var list = await _context.AspNetUsers.Where(x=>x.OrganizationId == user.OrganizationId).Select(x => new {
                x.Id,
                x.FullName
            })
                .ToListAsync();
            if (list == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "List of users not founded!" });
            }
            return Ok(list);
        }

    }
}
