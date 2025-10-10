using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un estado de análisis en el sistema SALC según ERS v2.7
    /// Tabla: estados_analisis
    /// </summary>
    public class EstadoAnalisis
    {
        #region Propiedades de la tabla estados_analisis
        /// <summary>
        /// ID único del estado (PK, IDENTITY)
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// Descripción del estado ('Sin verificar', 'Verificado')
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

        #region Métodos de validación
        /// <summary>
        /// Valida los datos del estado
        /// </summary>
        public bool EsValido()
        {
            return IdEstado > 0 && !string.IsNullOrWhiteSpace(Descripcion);
        }
        #endregion

        #region Sobrescritura de métodos base
        public override string ToString()
        {
            return Descripcion ?? "Estado sin descripción";
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