using Core.Entities;
using Core.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class PlanPromocionalRepository : IPlanPromocionalRepository
    {
        private readonly NpgsqlConnection _connection;

        public PlanPromocionalRepository()
        {
            _connection = DBContext.GetConnection();
        }

        public async Task CrearAsync(PlanPromocional plan)
        {
            await _connection.OpenAsync();

            // Insertar en Planes
            var insertPlan = new NpgsqlCommand(@"
                INSERT INTO Planes (nombre, fechainicio, fechafin, dcto)
                VALUES (@nombre, @inicio, @fin, @descuento)
                RETURNING id;", _connection);

            insertPlan.Parameters.AddWithValue("@nombre", plan.Nombre);
            insertPlan.Parameters.AddWithValue("@inicio", plan.FechaInicio);
            insertPlan.Parameters.AddWithValue("@fin", plan.FechaFin);
            insertPlan.Parameters.AddWithValue("@descuento", plan.Descuento);

            int planId = (int)await insertPlan.ExecuteScalarAsync();

            // Insertar en PlanProducto
            foreach (var producto in plan.Productos)
            {
                var insertProducto = new NpgsqlCommand(@"
                    INSERT INTO PlanProducto (plan_id, producto_id)
                    VALUES (@planId, @productoId);", _connection);

                insertProducto.Parameters.AddWithValue("@planId", planId);
                insertProducto.Parameters.AddWithValue("@productoId", producto.ProductoId);
                await insertProducto.ExecuteNonQueryAsync();
            }

            await _connection.CloseAsync();
        }

        public async Task<List<PlanPromocional>> ObtenerActivosAsync(DateTime fecha)
        {
            var planes = new List<PlanPromocional>();

            var query = @"
                SELECT id, nombre, fechainicio, fechafin, dcto
                FROM Planes
                WHERE @fecha BETWEEN fechainicio AND fechafin;";

            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@fecha", fecha.Date);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                planes.Add(new PlanPromocional
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    FechaInicio = reader.GetDateTime(2),
                    FechaFin = reader.GetDateTime(3),
                    Descuento = reader.GetDecimal(4)
                });
            }

            await _connection.CloseAsync();
            return planes;
        }
    }
}
