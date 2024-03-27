using APIAvtoMig.Auth;
using APIAvtoMig.Models;
using APIAvtoMig.Task.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIAvtoMig.Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private ApplicationDbContext context;
        public TaskController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [Route("Products")]
        public async Task<IActionResult> Products()
        {
            var list = await context.Product.ToListAsync();
            if (list == null)
            {
                return BadRequest();
            }

            return Ok(list);
        }
        [HttpPost]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var checkProduct = await context.Product.Where(x=>x.IsDeleted == false).
                FirstOrDefaultAsync(x=>x.Name == product.Name); // проверка на то, суещствует ли данный продукт в бд со свойством "удалено"
            if (checkProduct != null)
            {
                return BadRequest();
            }
            await context.Product.AddAsync(product);
            await context.SaveChangesAsync();
            return Ok(product);
        }
        [HttpPatch]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await context.Product.
                Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.IsDeleted = true;
            await context.SaveChangesAsync();
            return Ok();
        }
        [Route("EditProduct")]
        [HttpPatch]
        public async Task<IActionResult> EditProduct(Guid? id, [FromBody] Product? product)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var existingProduct = await context.Product.
                Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        [Route("FullDeleteProduct")]
        public async Task<IActionResult> FullDeleteProduct(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await context.Product.FirstOrDefaultAsync(x=>x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            context.Product.Remove(product);

            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
