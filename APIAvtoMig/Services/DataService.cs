using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIAvtoMig.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext context;
        public DataService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<object> GetDataWithId(int id, string context)
        {
            switch (context.ToLower())
            {
                case "user":
                    return await GetUserById(id);
                case "modelcar":
                    return await GetModelCarById(id);
                // Добавьте другие варианты по мере необходимости
                default:
                    return null; // или бросайте исключение BadRequest("Invalid context");
            }
        }

        public async Task<IEnumerable<object>> GetDataList(string context)
        {
            switch (context.ToLower())
            {
                case "users":
                    return await GetAllUsers();
                case "modelcars":
                    return await GetAllModelCars();
                // Добавьте другие варианты по мере необходимости
                default:
                    return null; // или бросайте исключение BadRequest("Invalid context");
            }
        }

        private async Task<AspNetUser> GetUserById(int id)
        {
            return null;
        }

        public async Task<ObjectResult> GetModelCarById(int? id)
        {
            if (id == null)
            {
                return new ObjectResult(new { Error = "Id is null" }) { StatusCode = 400 };
            }
            var modelCar = await context.Cars.Where(x=>x.Id == id).ToListAsync();
            if (modelCar == null)
            {
                return new ObjectResult(new { Error = $"ModelCar with Id {id} not found" }) { StatusCode = 404 };
            }
            return new ObjectResult(modelCar) { StatusCode = 200 };
        }

        private async Task<IEnumerable<AspNetUser>> GetAllUsers()
        {
            return null;
        }

        private async Task<IEnumerable<ModelCar>> GetAllModelCars()
        {
            return null;
        }







        /*public async Task<ModelCar> GetModelCarById(int? id)
        {
            var selectedCars = await context.ModelCars.Where(x=>x.CarId == id).ToListAsync();
            try
            {
                return await context.Cars.Where(x=>x.Id == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }*/
    }
}
