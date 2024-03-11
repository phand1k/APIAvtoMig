using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using APIAvtoMig.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIAvtoMig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        private ApplicationDbContext _context;
        private readonly CarService _carService;
        public DataController(ApplicationDbContext context, CarService carService)
        {
            _context = context;
            _carService = carService;
        }
    }
}
