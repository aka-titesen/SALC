// Models/TipoAnalisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un tipo de an�lisis en el sistema SALC seg�n ERS v2.7
    /// Tabla: tipos_analisis
    /// </summary>
    public class TipoAnalisis
    {
        #region Propiedades de la tabla tipos_analisis
        /// <summary>
        /// ID �nico del tipo de an�lisis (PK, IDENTITY)
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// Descripci�n del tipo de an�lisis
        /// </summary>
        public string Descripcion { get; set; }
        #endregion

        #region M�todos de validaci�n
        /// <summary>
        /// Valida los datos del tipo de an�lisis
        /// </summary>
        public bool EsValido()
        {
            return IdTipoAnalisis > 0 && !string.IsNullOrWhiteSpace(Descripcion);
        }
        #endregion

        #region Sobrescritura de m�todos base
        public override string ToString()
        {
            return Descripcion ?? "Tipo de an�lisis sin descripci�n";
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