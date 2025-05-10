namespace Core.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime Fecha { get; set; }
        public string DocumentoReferencia { get; set; } = string.Empty;
        public List<DetalleCompra> Detalles { get; set; } = new();
    }
}
