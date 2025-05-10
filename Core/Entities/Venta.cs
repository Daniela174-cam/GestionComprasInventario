using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Venta
{
    public int Id { get; set; }
    public string ClienteId { get; set; }    
    public string EmpleadoId { get; set; }   
    public DateTime Fecha { get; set; }
    public string DocumentoReferencia { get; set; } = "";
    public List<DetalleVenta> Detalles { get; set; } = new();
}
}
