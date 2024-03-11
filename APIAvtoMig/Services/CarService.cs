using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIAvtoMig.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace APIAvtoMig.Services
{
    public class CarService : IDataService<Car>
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
                return new BadRequestObjectResult(new Response { Status = "Error", Message = "Id is null!" });

            var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (car == null)
                return new NotFoundObjectResult(new Response { Status = "Error", Message = $"Car with {id} is not found!" });

            return new OkObjectResult(car);
        }

        public async Task<IActionResult> GetList()
        {
            var listCars = await _context.Cars.Where(x => x.IsDeleted == false).ToListAsync();

            if (!listCars.Any())
                return new BadRequestObjectResult(new Response { Status = "Error", Message = "Cars not found" });

            return new OkObjectResult(listCars);
        }

        public async Task<IActionResult> Create(Car entity)
        {
            var carExists = await _context.Cars.Where(x=>x.IsDeleted == false).FirstOrDefaultAsync(x => x.Name == entity.Name);

            if (carExists != null)
                return new ConflictObjectResult(new Response { Status = "Error", Message = "Car already exists!" });

            _context.Cars.Add(entity);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new Response { Status = "Success", Message = "Car created successfully!" });
        }

        public async Task<IActionResult> DeleteCar(int id)
        {
            var carExists = await _context.Cars
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ModelCars) // Подключаем связанные модели машин
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carExists == null)
                return new ConflictObjectResult(new Response { Status = "Error", Message = "Car does not exist!" });

            // Помечаем машину как удаленную
            carExists.IsDeleted = true;

            // Помечаем все связанные модели машин также как удаленные
            foreach (var modelCar in carExists.ModelCars)
            {
                modelCar.IsDeleted = true;
            }

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return new OkObjectResult(new Response { Status = "Success", Message = "Car and its models deleted successfully!" });
        }

    }
}
