using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPlanPromocionalRepository
    {
        Task CrearAsync(PlanPromocional plan);
        Task<List<PlanPromocional>> ObtenerActivosAsync(DateTime fecha);
    }
}
