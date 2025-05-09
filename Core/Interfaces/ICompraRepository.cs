using Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ICompraRepository
    {
        Task<List<Compra>> GetAllAsync();
        Task<Compra> GetByIdAsync(int id);
        Task AddAsync(Compra compra);
        Task DeleteAsync(int id);
    }
}
