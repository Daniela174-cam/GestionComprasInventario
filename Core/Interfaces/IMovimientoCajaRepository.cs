using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovimientoCajaRepository
    {
        Task RegistrarAsync(MovimientoCaja movimiento);
        Task<decimal> ObtenerBalanceDiarioAsync(DateTime fecha);
        Task<List<MovimientoCaja>> ObtenerPorFechaAsync(DateTime fecha);
    }
}
