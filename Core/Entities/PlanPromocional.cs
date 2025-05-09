using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class PlanPromocional
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Descuento { get; set; }

        // Relación con productos
        public List<PlanProducto> Productos { get; set; } = new();
    }
}
