using System;

namespace SALC.Models
{
    /// <summary>
    /// Clase base para todas las entidades del sistema SALC.
    /// Proporciona propiedades comunes y funcionalidad base.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Fecha de creaci�n del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha de �ltima modificaci�n del registro
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Indica si el registro est� activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// M�todo para validar la entidad
        /// </summary>
        /// <returns>True si la entidad es v�lida</returns>
        public virtual bool EsValido()
        {
            return true;
        }

        /// <summary>
        /// Actualiza la fecha de modificaci�n
        /// </summary>
        public virtual void ActualizarFechaModificacion()
        {
            FechaModificacion = DateTime.Now;
        }
    }
}