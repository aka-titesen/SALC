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
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha de última modificación del registro
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Método para validar la entidad
        /// </summary>
        /// <returns>True si la entidad es válida</returns>
        public virtual bool EsValido()
        {
            return true;
        }

        /// <summary>
        /// Actualiza la fecha de modificación
        /// </summary>
        public virtual void ActualizarFechaModificacion()
        {
            FechaModificacion = DateTime.Now;
        }
    }
}