using Core.Entities;
using Core.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class VentaRepository : IVentaRepository
    {
        private readonly NpgsqlConnection _connection;

        public VentaRepository()
        {
            _connection = DBContext.GetConnection();
        }

        public async Task<List<Venta>> GetAllAsync()
        {
            var ventas = new List<Venta>();
            string query = "SELECT fact_id, tercerocli_id, terceroem_id, fecha, docventa FROM Ventas";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ventas.Add(new Venta
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1).ToString(),
                    EmpleadoId = reader.GetInt32(2).ToString(),
                    Fecha = reader.GetDateTime(3),
                    DocumentoReferencia = reader.GetString(4)
                });
            }

            await _connection.CloseAsync();
            return ventas;
        }

        public async Task<Venta> GetByIdAsync(int id)
        {
            Venta venta = null;
            string query = "SELECT fact_id, tercerocli_id, terceroem_id, fecha, docventa FROM Ventas WHERE fact_id = @id";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                venta = new Venta
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1).ToString(),
                    EmpleadoId = reader.GetInt32(2).ToString(),
                    Fecha = reader.GetDateTime(3),
                    DocumentoReferencia = reader.GetString(4)
                };
            }

            await _connection.CloseAsync();
            return venta;
        }

        public async Task AddAsync(Venta venta)
{
    await _connection.OpenAsync();

    var insertVenta = new NpgsqlCommand(@"
        INSERT INTO Ventas (tercerocli_id, terceroem_id, fecha, docventa)
        VALUES (@cliente, @empleado, @fecha, @doc)
        RETURNING fact_id;", _connection);

    insertVenta.Parameters.AddWithValue("@cliente", int.Parse(venta.ClienteId));
    insertVenta.Parameters.AddWithValue("@empleado", int.Parse(venta.EmpleadoId));
    insertVenta.Parameters.AddWithValue("@fecha", venta.Fecha);
    insertVenta.Parameters.AddWithValue("@doc", venta.DocumentoReferencia);

    int ventaId = (int)await insertVenta.ExecuteScalarAsync();

    foreach (var detalle in venta.Detalles)
    {
        var insertDetalle = new NpgsqlCommand(@"
            INSERT INTO Detalle_Venta (producto_id, cantidad, valor, fecha, fact_id)
            VALUES (@producto, @cantidad, @valor, @fecha, @fact_id);", _connection);

        insertDetalle.Parameters.AddWithValue("@producto", int.Parse(detalle.ProductoId));
        insertDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
        insertDetalle.Parameters.AddWithValue("@valor", detalle.ValorUnitario);
        insertDetalle.Parameters.AddWithValue("@fecha", DateTime.Now);
        insertDetalle.Parameters.AddWithValue("@fact_id", ventaId);

        await insertDetalle.ExecuteNonQueryAsync();

        var updateStock = new NpgsqlCommand(@"
            UPDATE Productos
            SET stock = stock - @cantidad, updatedat = @now
            WHERE id = @producto;", _connection);

        updateStock.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
        updateStock.Parameters.AddWithValue("@now", DateTime.Now);
        updateStock.Parameters.AddWithValue("@producto", detalle.ProductoId.ToString());


        await updateStock.ExecuteNonQueryAsync();
    }

    await _connection.CloseAsync();
}


        public async Task DeleteAsync(int id)
        {
            await _connection.OpenAsync();

            var deleteDetalles = new NpgsqlCommand("DELETE FROM Detalle_Venta WHERE fact_id = @id", _connection);
            deleteDetalles.Parameters.AddWithValue("@id", id);
            await deleteDetalles.ExecuteNonQueryAsync();

            var deleteVenta = new NpgsqlCommand("DELETE FROM Ventas WHERE fact_id = @id", _connection);
            deleteVenta.Parameters.AddWithValue("@id", id);
            await deleteVenta.ExecuteNonQueryAsync();

            await _connection.CloseAsync();
        }
    }
}
