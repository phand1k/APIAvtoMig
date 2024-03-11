using APIAvtoMig.Auth;
using APIAvtoMig.Interfaces;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIAvtoMig.Services
{
    public class ModelCarService : IDataService<ModelCar>
    {
        private readonly ApplicationDbContext _context;

        public ModelCarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
                return new BadRequestObjectResult(new Response { Status = "Error", Message = "Id is null!" });

            var car = await _context.ModelCars.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (car == null)
                return new NotFoundObjectResult(new Response { Status = "Error", Message = $"Model car with {id} is not found!" });

            return new OkObjectResult(car);
        }

        public async Task<IActionResult> GetList()
        {
            var listCars = await _context.ModelCars.Where(x => x.IsDeleted == false).ToListAsync();

            if (!listCars.Any())
                return new BadRequestObjectResult(new Response { Status = "Error", Message = "Model cars not found" });

            return new OkObjectResult(listCars);
        }

        public async Task<IActionResult> Create(ModelCar entity)
        {
            var carExists = await _context.ModelCars.FirstOrDefaultAsync(x => x.Name == entity.Name && x.IsDeleted == false);

            if (carExists != null)
                return new ConflictObjectResult(new Response { Status = "Error", Message = "Model car already exists!" });

            _context.ModelCars.Add(entity);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new Response { Status = "Success", Message = "Model car created successfully!" });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var modelCarExists = await _context.ModelCars
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (modelCarExists == null)
                return new ConflictObjectResult(new Response { Status = "Error", Message = "Model car does not exist!" });

            modelCarExists.IsDeleted = true;


            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return new OkObjectResult(new Response { Status = "Success", Message = "Model car and its models deleted successfully!" });
        }
    }
}
