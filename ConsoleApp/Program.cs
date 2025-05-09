using System;
using Core.Entities;
using Infrastructure.Persistence.PostgreSQL;
using Infrastructure.Factories;

class Program
{
    static async Task Main(string[] args)
    {
        var productoRepo = new ProductoRepository();
        var compraRepo = new CompraRepository();
        var movimientoRepo = new MovimientoCajaRepository();
        var planRepo = new PlanPromocionalRepository();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== MENÚ PRINCIPAL =====");
            Console.WriteLine("1. Registrar compra");
            Console.WriteLine("2. Salir");
            Console.WriteLine("3. Registrar movimiento de caja");
            Console.WriteLine("4. Consultar balance diario");
            Console.WriteLine("5. Crear plan promocional");
            Console.WriteLine("6. Ver planes activos por fecha");
            Console.WriteLine("7. Registrar venta");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    var compra = new Compra();
                    Console.Write("ID proveedor: "); compra.ProveedorId = Console.ReadLine();
                    Console.Write("ID empleado: "); compra.EmpleadoId = Console.ReadLine();
                    Console.Write("Documento ref: "); compra.DocumentoReferencia = Console.ReadLine();
                    compra.Fecha = DateTime.Now;

                    Console.Write("¿Cuántos productos desea agregar? ");
                    int cantidad = int.Parse(Console.ReadLine());

                    for (int i = 0; i < cantidad; i++)
                    {
                        var detalle = new DetalleCompra();
                        Console.Write($"ID producto #{i + 1}: "); detalle.ProductoId = Console.ReadLine();
                        Console.Write($"Cantidad: "); detalle.Cantidad = int.Parse(Console.ReadLine());
                        Console.Write($"Valor unitario: "); detalle.ValorUnitario = decimal.Parse(Console.ReadLine());
                        compra.Detalles.Add(detalle);
                    }

                    await compraRepo.AddAsync(compra);
                    Console.WriteLine("\n✅ Compra registrada y stock actualizado.");
                    break;

                case "3":
                    Console.Write("Tipo de movimiento (Entrada/Salida): ");
                    string tipo = Console.ReadLine();
                    Console.Write("ID tipo movimiento (FK en TipoMovCaja): ");
                    int tipoMovId = int.Parse(Console.ReadLine());
                    Console.Write("Valor: ");
                    decimal valor = decimal.Parse(Console.ReadLine());
                    Console.Write("Concepto: ");
                    string concepto = Console.ReadLine();
                    Console.Write("ID Tercero (cliente/proveedor/empleado): ");
                    string terceroId = Console.ReadLine();

                    try
                    {
                        var movimiento = MovimientoFactory.CrearMovimiento(tipo, tipoMovId, valor, concepto, terceroId);
                        await movimientoRepo.RegistrarAsync(movimiento);
                        Console.WriteLine("\n✅ Movimiento registrado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error: {ex.Message}");
                    }
                    break;

                case "4":
                    Console.Write("Ingrese fecha (yyyy-mm-dd): ");
                    DateTime fecha = DateTime.Parse(Console.ReadLine());
                    decimal balance = await movimientoRepo.ObtenerBalanceDiarioAsync(fecha);
                    Console.WriteLine($"\n💰 Balance del {fecha:yyyy-MM-dd}: {balance}");
                    break;

                case "5":
                    var plan = new PlanPromocional();
                    Console.Write("Nombre del plan: "); plan.Nombre = Console.ReadLine();
                    Console.Write("Fecha inicio (yyyy-mm-dd): "); plan.FechaInicio = DateTime.Parse(Console.ReadLine());
                    Console.Write("Fecha fin (yyyy-mm-dd): "); plan.FechaFin = DateTime.Parse(Console.ReadLine());
                    Console.Write("Descuento (%): "); plan.Descuento = decimal.Parse(Console.ReadLine());

                    Console.Write("¿Cuántos productos tendrá el plan?: ");
                    int cantidadProd = int.Parse(Console.ReadLine());

                    for (int i = 0; i < cantidadProd; i++)
                    {
                        Console.Write($"ID producto #{i + 1}: ");
                        var idProd = Console.ReadLine();
                        plan.Productos.Add(new PlanProducto { ProductoId = idProd });
                    }

                    await planRepo.CrearAsync(plan);
                    Console.WriteLine("\n✅ Plan promocional creado correctamente.");
                    break;

                case "6":
                    Console.Write("Fecha para consultar (yyyy-mm-dd): ");
                    var fechaConsulta = DateTime.Parse(Console.ReadLine());

                    var activos = await planRepo.ObtenerActivosAsync(fechaConsulta);

                    Console.WriteLine("\n📋 Planes activos:");
                    foreach (var p in activos)
                    {
                        Console.WriteLine($"- {p.Nombre} ({p.FechaInicio:yyyy-MM-dd} a {p.FechaFin:yyyy-MM-dd}) - Dcto: {p.Descuento}%");
                    }
                    break;

                case "7":
                    var venta = new Venta();
                    Console.Write("ID cliente: "); venta.ClienteId = Console.ReadLine();
                    Console.Write("ID empleado: "); venta.EmpleadoId = Console.ReadLine();
                    Console.Write("Documento ref: "); venta.DocumentoReferencia = Console.ReadLine();
                    venta.Fecha = DateTime.Now;

                    Console.Write("¿Cuántos productos desea vender? ");
                    int cantidadVenta = int.Parse(Console.ReadLine());

                    decimal totalVenta = 0;

                    for (int i = 0; i < cantidadVenta; i++)
                    {
                        var detalle = new DetalleVenta();
                        Console.Write($"ID producto #{i + 1}: "); detalle.ProductoId = Console.ReadLine();
                        Console.Write("Cantidad: "); detalle.Cantidad = int.Parse(Console.ReadLine());
                        Console.Write("Valor unitario: "); detalle.ValorUnitario = decimal.Parse(Console.ReadLine());
                        totalVenta += detalle.Cantidad * detalle.ValorUnitario;
                        venta.Detalles.Add(detalle);
                    }

                    var ventaRepo = new VentaRepository();
                    await ventaRepo.AddAsync(venta);
                    Console.WriteLine("✅ Venta registrada y stock actualizado.");

                    // Registrar movimiento de caja tipo Entrada
                    try
                    {
                        var movimiento = MovimientoFactory.CrearMovimiento(
                            "Entrada",
                            tipoMovimientoId: 1, // Asumiendo que '1' es Entrada por venta
                            valor: totalVenta,
                            concepto: $"Venta registrada: {venta.DocumentoReferencia}",
                            terceroId: venta.ClienteId
                        );

                        await movimientoRepo.RegistrarAsync(movimiento);
                        Console.WriteLine("✅ Movimiento de caja registrado (Entrada por venta).");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ No se pudo registrar movimiento de caja: {ex.Message}");
                    }

                    break;
            }
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}