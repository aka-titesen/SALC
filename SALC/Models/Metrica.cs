// Models/Metrica.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa una métrica para análisis en el sistema SALC según ERS v2.7
    /// Tabla: metricas
    /// </summary>
    public class Metrica
    {
        #region Propiedades de la tabla metricas
        /// <summary>
        /// ID único de la métrica (PK, IDENTITY)
        /// </summary>
        public int IdMetrica { get; set; }

        /// <summary>
        /// Nombre de la métrica (ej. 'Glucosa', 'Colesterol Total')
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Unidad de medida (ej. 'mg/dL', 'g/dL')
        /// </summary>
        public string UnidadMedida { get; set; }

        /// <summary>
        /// Valor de referencia máximo (nullable)
        /// </summary>
        public decimal? ValorMaximo { get; set; }

        /// <summary>
        /// Valor de referencia mínimo (nullable)
        /// </summary>
        public decimal? ValorMinimo { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Rango de referencia formateado
        /// </summary>
        public string RangoReferencia
        {
            get
            {
                if (ValorMinimo.HasValue && ValorMaximo.HasValue)
                    return $"{ValorMinimo.Value} - {ValorMaximo.Value} {UnidadMedida}";
                else if (ValorMinimo.HasValue)
                    return $"> {ValorMinimo.Value} {UnidadMedida}";
                else if (ValorMaximo.HasValue)
                    return $"< {ValorMaximo.Value} {UnidadMedida}";
                else
                    return "Sin rango definido";
            }
        }

        /// <summary>
        /// Descripción completa de la métrica
        /// </summary>
        public string DescripcionCompleta => $"{Nombre} ({UnidadMedida})";

        /// <summary>
        /// Verifica si tiene rango de referencia definido
        /// </summary>
        public bool TieneRangoReferencia => ValorMinimo.HasValue || ValorMaximo.HasValue;
        #endregion

        #region Métodos de validación
        /// <summary>
        /// Valida los datos de la métrica
        /// </summary>
        public bool EsValida()
        {
            return IdMetrica > 0 &&
                   !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(UnidadMedida) &&
                   (!ValorMinimo.HasValue || !ValorMaximo.HasValue || ValorMinimo.Value <= ValorMaximo.Value);
        }

        /// <summary>
        /// Evalúa si un valor está dentro del rango de referencia
        /// </summary>
        /// <param name="valor">Valor a evaluar</param>
        /// <returns>True si está en rango, False si está fuera, null si no hay rango definido</returns>
        public bool? EstaEnRango(decimal valor)
        {
            if (!TieneRangoReferencia) return null;

            bool dentroDelMinimo = !ValorMinimo.HasValue || valor >= ValorMinimo.Value;
            bool dentroDelMaximo = !ValorMaximo.HasValue || valor <= ValorMaximo.Value;

            return dentroDelMinimo && dentroDelMaximo;
        }

        /// <summary>
        /// Obtiene el estado del valor respecto al rango
        /// </summary>
        /// <param name="valor">Valor a evaluar</param>
        /// <returns>Estado del valor</returns>
        public string ObtenerEstadoValor(decimal valor)
        {
            var enRango = EstaEnRango(valor);
            
            if (!enRango.HasValue) return "Sin referencia";
            if (enRango.Value) return "Normal";

            if (ValorMinimo.HasValue && valor < ValorMinimo.Value) return "Bajo";
            if (ValorMaximo.HasValue && valor > ValorMaximo.Value) return "Alto";

            return "Fuera de rango";
        }
        #endregion

        #region Sobrescritura de métodos base
        public override string ToString()
        {
            return DescripcionCompleta;
        }

        public override bool Equals(object obj)
        {
            if (obj is Metrica other)
                return IdMetrica == other.IdMetrica;
            return false;
        }

        public override int GetHashCode()
        {
            return IdMetrica.GetHashCode();
        }
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use IdMetrica instead")]
        public int IdMet
        {
            get => IdMetrica;
            set => IdMetrica = value;
        }

        [Obsolete("Use UnidadMedida instead")]
        public string Unidad
        {
            get => UnidadMedida;
            set => UnidadMedida = value;
        }
        #endregion
    }
}