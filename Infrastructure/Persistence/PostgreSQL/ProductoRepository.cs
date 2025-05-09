using Core.Entities;
using Core.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly NpgsqlConnection _connection;

        public ProductoRepository()
        {
            _connection = DBContext.GetConnection();
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            var productos = new List<Producto>();
            var query = "SELECT * FROM Productos";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetString(0),
                    Nombre = reader.GetString(1),
                    Stock = reader.GetInt32(2),
                    StockMin = reader.GetInt32(3),
                    StockMax = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6),
                    Barcode = reader.GetString(7)
                });
            }

            await _connection.CloseAsync();
            return productos;
        }

        public async Task<Producto> GetByIdAsync(string id)
        {
            Producto producto = null;
            var query = "SELECT * FROM Productos WHERE id = @id";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                producto = new Producto
                {
                    Id = reader.GetString(0),
                    Nombre = reader.GetString(1),
                    Stock = reader.GetInt32(2),
                    StockMin = reader.GetInt32(3),
                    StockMax = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6),
                    Barcode = reader.GetString(7)
                };
            }

            await _connection.CloseAsync();
            return producto;
        }

        public async Task AddAsync(Producto producto)
        {
            var query = @"INSERT INTO Productos 
                (id, nombre, stock, stockmin, stockmax, createdat, updatedat, barcode)
                VALUES (@id, @nombre, @stock, @stockmin, @stockmax, @createdat, @updatedat, @barcode)";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", producto.Id);
            cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@stock", producto.Stock);
            cmd.Parameters.AddWithValue("@stockmin", producto.StockMin);
            cmd.Parameters.AddWithValue("@stockmax", producto.StockMax);
            cmd.Parameters.AddWithValue("@createdat", producto.CreatedAt);
            cmd.Parameters.AddWithValue("@updatedat", producto.UpdatedAt);
            cmd.Parameters.AddWithValue("@barcode", producto.Barcode);
            await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            var query = @"UPDATE Productos SET
                nombre = @nombre, stock = @stock, stockmin = @stockmin,
                stockmax = @stockmax, updatedat = @updatedat, barcode = @barcode
                WHERE id = @id";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", producto.Id);
            cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@stock", producto.Stock);
            cmd.Parameters.AddWithValue("@stockmin", producto.StockMin);
            cmd.Parameters.AddWithValue("@stockmax", producto.StockMax);
            cmd.Parameters.AddWithValue("@updatedat", producto.UpdatedAt);
            cmd.Parameters.AddWithValue("@barcode", producto.Barcode);
            await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var query = "DELETE FROM Productos WHERE id = @id";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }
    }
}
