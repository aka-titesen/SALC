// Models/AnalisisMetrica.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa la relación entre un análisis y sus métricas con valores según ERS v2.7
    /// Tabla: analisis_metrica
    /// </summary>
    public class AnalisisMetrica
    {
        #region Propiedades de la tabla analisis_metrica
        /// <summary>
        /// ID del análisis (PK, FK a analisis.id_analisis)
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// ID de la métrica (PK, FK a metricas.id_metrica)
        /// </summary>
        public int IdMetrica { get; set; }

        /// <summary>
        /// Valor del resultado numérico
        /// </summary>
        public decimal Resultado { get; set; }

        /// <summary>
        /// Observaciones específicas del resultado (opcional)
        /// </summary>
        public string Observaciones { get; set; }
        #endregion

        #region Propiedades de navegación
        /// <summary>
        /// Análisis asociado
        /// </summary>
        public Analisis Analisis { get; set; }

        /// <summary>
        /// Métrica asociada
        /// </summary>
        public Metrica Metrica { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Estado del resultado respecto al rango de referencia
        /// </summary>
        public string EstadoResultado => Metrica?.ObtenerEstadoValor(Resultado) ?? "Sin referencia";

        /// <summary>
        /// Verifica si el resultado está fuera del rango normal
        /// </summary>
        public bool EstaFueraDeRango
        {
            get
            {
                var enRango = Metrica?.EstaEnRango(Resultado);
                return enRango.HasValue && !enRango.Value;
            }
        }

        /// <summary>
        /// Resultado formateado con unidad de medida
        /// </summary>
        public string ResultadoFormateado => Metrica != null 
            ? $"{Resultado} {Metrica.UnidadMedida}" 
            : Resultado.ToString();

        /// <summary>
        /// Descripción completa del resultado
        /// </summary>
        public string DescripcionCompleta => Metrica != null
            ? $"{Metrica.Nombre}: {ResultadoFormateado} ({EstadoResultado})"
            : $"Resultado: {Resultado}";

        /// <summary>
        /// Rango de referencia de la métrica
        /// </summary>
        public string RangoReferencia => Metrica?.RangoReferencia ?? "Sin rango definido";
        #endregion

        #region Métodos de validación
        /// <summary>
        /// Valida los datos del resultado
        /// </summary>
        public bool EsValido()
        {
            return IdAnalisis > 0 && 
                   IdMetrica > 0 && 
                   Resultado >= 0; // Los resultados no pueden ser negativos
        }

        /// <summary>
        /// Verifica si el resultado requiere atención médica
        /// </summary>
        public bool RequiereAtencion()
        {
            return EstaFueraDeRango;
        }
        #endregion

        #region Sobrescritura de métodos base
        public override string ToString()
        {
            return DescripcionCompleta;
        }

        public override bool Equals(object obj)
        {
            if (obj is AnalisisMetrica other)
                return IdAnalisis == other.IdAnalisis && IdMetrica == other.IdMetrica;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdAnalisis, IdMetrica);
        }
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use Resultado instead")]
        public decimal Valor
        {
            get => Resultado;
            set => Resultado = value;
        }
        #endregion
    }
}