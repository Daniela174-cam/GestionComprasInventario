using Core.Entities;
using Core.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class MovimientoCajaRepository : IMovimientoCajaRepository
    {
        private readonly NpgsqlConnection _connection;

        public MovimientoCajaRepository()
        {
            _connection = DBContext.GetConnection();
        }

        public async Task RegistrarAsync(MovimientoCaja movimiento)
        {
            var query = @"
        INSERT INTO MovCaja (fecha, tipoMov_id, valor, concepto, tercero_id)
        VALUES (@fecha, @tipo, @valor, @concepto, @tercero)";

            if (_connection.State != System.Data.ConnectionState.Open)
                await _connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@fecha", movimiento.Fecha);
            cmd.Parameters.AddWithValue("@tipo", movimiento.TipoMovimientoId);
            cmd.Parameters.AddWithValue("@valor", movimiento.Valor);
            cmd.Parameters.AddWithValue("@concepto", movimiento.Concepto);
            cmd.Parameters.AddWithValue("@tercero", movimiento.TerceroId);
            await cmd.ExecuteNonQueryAsync();

            if (_connection.State == System.Data.ConnectionState.Open)
                await _connection.CloseAsync();
        }


        public async Task<List<MovimientoCaja>> ObtenerPorFechaAsync(DateTime fecha)
        {
            var movimientos = new List<MovimientoCaja>();
            var query = "SELECT id, fecha, tipoMov_id, valor, concepto, tercero_id FROM MovCaja WHERE fecha = @fecha";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@fecha", fecha.Date);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movimientos.Add(new MovimientoCaja
                {
                    Id = reader.GetInt32(0),
                    Fecha = reader.GetDateTime(1),
                    TipoMovimientoId = reader.GetInt32(2),
                    Valor = reader.GetDecimal(3),
                    Concepto = reader.GetString(4),
                    TerceroId = reader.GetString(5)
                });
            }

            await _connection.CloseAsync();
            return movimientos;
        }

        public async Task<decimal> ObtenerBalanceDiarioAsync(DateTime fecha)
        {
            decimal entradas = 0, salidas = 0;

            var entradaQuery = @"
                SELECT COALESCE(SUM(valor), 0) FROM MovCaja 
                JOIN TipoMovCaja ON MovCaja.tipoMov_id = TipoMovCaja.id 
                WHERE fecha = @fecha AND TipoMovCaja.tipo = 'Entrada'";

            var salidaQuery = @"
                SELECT COALESCE(SUM(valor), 0) FROM MovCaja 
                JOIN TipoMovCaja ON MovCaja.tipoMov_id = TipoMovCaja.id 
                WHERE fecha = @fecha AND TipoMovCaja.tipo = 'Salida'";

            await _connection.OpenAsync();

            using (var cmd = new NpgsqlCommand(entradaQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                entradas = (decimal)await cmd.ExecuteScalarAsync();
            }

            using (var cmd = new NpgsqlCommand(salidaQuery, _connection))
            {
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                salidas = (decimal)await cmd.ExecuteScalarAsync();
            }

            await _connection.CloseAsync();
            return entradas - salidas;
        }
    }
}
