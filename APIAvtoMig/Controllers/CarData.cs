using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using APIAvtoMig.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIAvtoMig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarData : Controller
    {
        private ApplicationDbContext _context;
        private readonly CarService _carService;
        private readonly ModelCarService _modelCarService;
        public CarData(CarService _carService, ApplicationDbContext _context, ModelCarService _modelCarService)
        {
            this._carService = _carService;
            this._context = _context;
            this._modelCarService = _modelCarService;
        }

        [HttpPost("CreateModelCar")]
        public async Task<IActionResult> CreateModelCar([FromBody] ModelCar modelCar)
        {
            return await _modelCarService.Create(modelCar);
        }
        [HttpPost("DeleteModelCar")]
        public async Task<IActionResult> DeleteModelCar(int id)
        {
            return await _modelCarService.Delete(id);
        }
        [HttpGet("Cars/{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            return await _carService.GetById(id);
        }

        [HttpGet("Cars")]
        public async Task<IActionResult> GetCars()
        {
            return await _carService.GetList();
        }

        [HttpPost("CreateCar")]
        public async Task<IActionResult> CreateCar([FromBody] Car car)
        {
            return await _carService.Create(car);
        }
        [HttpPost]
        [Route("DeleteCar")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            return await _carService.DeleteCar(id);
        }
    }
}
