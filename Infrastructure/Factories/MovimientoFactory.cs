using Core.Entities;
using System;

namespace Infrastructure.Factories
{
    public static class MovimientoFactory
    {
        public static MovimientoCaja CrearMovimiento(
            string tipoMovimiento,    // "Entrada" o "Salida"
            int tipoMovimientoId,     // FK a TipoMovCaja
            decimal valor,
            string concepto,
            string terceroId)
        {
            if (tipoMovimiento != "Entrada" && tipoMovimiento != "Salida")
            {
                throw new ArgumentException("Tipo de movimiento inválido. Solo se permite 'Entrada' o 'Salida'.");
            }

            return new MovimientoCaja
            {
                Fecha = DateTime.Now,
                TipoMovimientoId = tipoMovimientoId,
                Valor = valor,
                Concepto = concepto,
                TerceroId = terceroId
            };
        }
    }
}

