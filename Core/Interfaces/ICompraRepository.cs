using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

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
