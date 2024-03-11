using Microsoft.AspNetCore.Mvc;

namespace APIAvtoMig.Interfaces
{
    public interface IDataService<T>
    {
        Task<IActionResult> GetById(int? id);
        Task<IActionResult> GetList();
        Task<IActionResult> Create(T entity);
    }
}
