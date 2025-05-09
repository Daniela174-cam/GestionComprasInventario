using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IVentaRepository
    {
        Task<List<Venta>> GetAllAsync();
        Task<Venta> GetByIdAsync(int id);
        Task AddAsync(Venta venta);
        Task DeleteAsync(int id);
    }
}
