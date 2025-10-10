using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un estado de an�lisis en el sistema SALC seg�n ERS v2.7
    /// Tabla: estados_analisis
    /// </summary>
    public class EstadoAnalisis
    {
        #region Propiedades de la tabla estados_analisis
        /// <summary>
        /// ID �nico del estado (PK, IDENTITY)
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// Descripci�n del estado ('Sin verificar', 'Verificado')
        /// </summary>
        public string Descripcion { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Verifica si es el estado "Sin verificar" (id = 1)
        /// </summary>
        public bool EsSinVerificar => IdEstado == 1;

        /// <summary>
        /// Verifica si es el estado "Verificado" (id = 2)
        /// </summary>
        public bool EsVerificado => IdEstado == 2;
        #endregion

        #region M�todos de validaci�n
        /// <summary>
        /// Valida los datos del estado
        /// </summary>
        public bool EsValido()
        {
            return IdEstado > 0 && !string.IsNullOrWhiteSpace(Descripcion);
        }
        #endregion

        #region Sobrescritura de m�todos base
        public override string ToString()
        {
            return Descripcion ?? "Estado sin descripci�n";
        }

        public override bool Equals(object obj)
        {
            if (obj is EstadoAnalisis other)
                return IdEstado == other.IdEstado;
            return false;
        }

        public override int GetHashCode()
        {
            return IdEstado.GetHashCode();
        }
        #endregion
    }
}