// Models/TipoAnalisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un tipo de análisis en el sistema SALC según ERS v2.7
    /// Tabla: tipos_analisis
    /// </summary>
    public class TipoAnalisis
    {
        #region Propiedades de la tabla tipos_analisis
        /// <summary>
        /// ID único del tipo de análisis (PK, IDENTITY)
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// Descripción del tipo de análisis
        /// </summary>
        public string Descripcion { get; set; }
        #endregion

        #region Métodos de validación
        /// <summary>
        /// Valida los datos del tipo de análisis
        /// </summary>
        public bool EsValido()
        {
            return IdTipoAnalisis > 0 && !string.IsNullOrWhiteSpace(Descripcion);
        }
        #endregion

        #region Sobrescritura de métodos base
        public override string ToString()
        {
            return Descripcion ?? "Tipo de análisis sin descripción";
        }

        public override bool Equals(object obj)
        {
            if (obj is TipoAnalisis other)
                return IdTipoAnalisis == other.IdTipoAnalisis;
            return false;
        }

        public override int GetHashCode()
        {
            return IdTipoAnalisis.GetHashCode();
        }
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use IdTipoAnalisis instead")]
        public int IdTipo
        {
            get => IdTipoAnalisis;
            set => IdTipoAnalisis = value;
        }
        #endregion
    }
}