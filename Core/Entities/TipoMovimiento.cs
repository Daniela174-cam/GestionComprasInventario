namespace Core.Entities
{
    public class TipoMovimiento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }   // Ej: "Pago Proveedor", "Ingreso por venta"
        public string Tipo { get; set; }     // "Entrada" o "Salida"
    }
}
