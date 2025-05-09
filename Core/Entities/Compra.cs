using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public string ProveedorId { get; set; }
        public string EmpleadoId { get; set; }
        public DateTime Fecha { get; set; }
        public string DocumentoReferencia { get; set; }
        public List<DetalleCompra> Detalles { get; set; } = new();
    }
}
