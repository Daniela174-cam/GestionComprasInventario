using Core.Entities;
using Core.Interfaces;
using Npgsql;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public class CompraRepository : ICompraRepository
{
    private readonly NpgsqlConnection _connection;

    public CompraRepository()
    {
        _connection = DBContext.GetConnection();
    }

    public async Task<List<Compra>> GetAllAsync()
    {
        var compras = new List<Compra>();
        string query = "SELECT id, terceroprov_id, terceroemp_id, fecha, doccompra FROM Compras";

        await _connection.OpenAsync();
        using var cmd = new NpgsqlCommand(query, _connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            compras.Add(new Compra
            {
                Id = reader.GetInt32(0),
                ProveedorId = reader.GetInt32(1),
                EmpleadoId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                DocumentoReferencia = reader.GetString(4)
            });
        }

        await _connection.CloseAsync();
        return compras;
    }

    public async Task<Compra> GetByIdAsync(int id)
    {
        Compra compra = null;
        string query = "SELECT id, terceroprov_id, terceroemp_id, fecha, doccompra FROM Compras WHERE id = @id";

        await _connection.OpenAsync();
        using var cmd = new NpgsqlCommand(query, _connection);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            compra = new Compra
            {
                Id = reader.GetInt32(0),
                ProveedorId = reader.GetInt32(1),
                EmpleadoId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                DocumentoReferencia = reader.GetString(4)
            };
        }

        await _connection.CloseAsync();
        return compra;
    }

    public async Task AddAsync(Compra compra)
    {
        await _connection.OpenAsync();

        var compraCmd = new NpgsqlCommand(@"
            INSERT INTO Compras (terceroprov_id, terceroemp_id, fecha, doccompra)
            VALUES (@prov, @emp, @fecha, @doc)
            RETURNING id", _connection);

        compraCmd.Parameters.AddWithValue("@prov", compra.ProveedorId);
        compraCmd.Parameters.AddWithValue("@emp", compra.EmpleadoId);
        compraCmd.Parameters.AddWithValue("@fecha", compra.Fecha);
        compraCmd.Parameters.AddWithValue("@doc", compra.DocumentoReferencia);

        var compraId = (int)await compraCmd.ExecuteScalarAsync();

        foreach (var detalle in compra.Detalles)
        {
            var detalleCmd = new NpgsqlCommand(@"
                INSERT INTO Detalle_Compra (producto_id, cantidad, valor, fecha, compra_id)
                VALUES (@producto, @cantidad, @valor, @fecha, @compra)", _connection);

            detalleCmd.Parameters.AddWithValue("@producto", detalle.ProductoId);
            detalleCmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            detalleCmd.Parameters.AddWithValue("@valor", detalle.ValorUnitario);
            detalleCmd.Parameters.AddWithValue("@fecha", DateTime.Now);
            detalleCmd.Parameters.AddWithValue("@compra", compraId);
            await detalleCmd.ExecuteNonQueryAsync();

            var updateStock = new NpgsqlCommand(@"
                UPDATE Productos SET stock = stock + @cantidad, updatedat = @now WHERE id = @producto", _connection);

            updateStock.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            updateStock.Parameters.AddWithValue("@now", DateTime.Now);
            updateStock.Parameters.AddWithValue("@producto", detalle.ProductoId);
            await updateStock.ExecuteNonQueryAsync();
        }

        await _connection.CloseAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _connection.OpenAsync();

        var deleteDetalles = new NpgsqlCommand("DELETE FROM Detalle_Compra WHERE compra_id = @id", _connection);
        deleteDetalles.Parameters.AddWithValue("@id", id);
        await deleteDetalles.ExecuteNonQueryAsync();

        var deleteCompra = new NpgsqlCommand("DELETE FROM Compras WHERE id = @id", _connection);
        deleteCompra.Parameters.AddWithValue("@id", id);
        await deleteCompra.ExecuteNonQueryAsync();

        await _connection.CloseAsync();
    }
}
