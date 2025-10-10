// Models/Rol.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un rol de usuario en el sistema SALC seg�n ERS v2.7
    /// Tabla: roles
    /// </summary>
    public class Rol
    {
        #region Propiedades de la tabla roles
        /// <summary>
        /// ID �nico del rol (PK, IDENTITY)
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del rol ('Administrador', 'M�dico', 'Asistente')
        /// </summary>
        public string NombreRol { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Verifica si es el rol de Administrador (id = 1)
        /// </summary>
        public bool EsAdministrador => IdRol == 1;

        /// <summary>
        /// Verifica si es el rol de M�dico (id = 2)
        /// </summary>
        public bool EsMedico => IdRol == 2;

        /// <summary>
        /// Verifica si es el rol de Asistente (id = 3)
        /// </summary>
        public bool EsAsistente => IdRol == 3;

        /// <summary>
        /// Descripci�n del nivel de acceso del rol
        /// </summary>
        public string DescripcionAcceso
        {
            get
            {
                return IdRol switch
                {
                    1 => "Acceso completo a todas las funcionalidades del sistema",
                    2 => "Crear an�lisis, cargar resultados, validar y generar informes",
                    3 => "Visualizar pacientes, generar informes de an�lisis verificados",
                    _ => "Permisos no definidos"
                };
            }
        }
        #endregion

        #region M�todos de validaci�n
        /// <summary>
        /// Valida los datos del rol
        /// </summary>
        public bool EsValido()
        {
            return IdRol > 0 && 
                   !string.IsNullOrWhiteSpace(NombreRol) &&
                   (EsAdministrador || EsMedico || EsAsistente);
        }

        /// <summary>
        /// Verifica si el rol tiene permisos para una operaci�n espec�fica
        /// </summary>
        /// <param name="operacion">Nombre de la operaci�n</param>
        /// <returns>True si tiene permisos</returns>
        public bool TienePermiso(string operacion)
        {
            if (EsAdministrador) return true; // Administrador tiene todos los permisos

            return operacion?.ToLowerInvariant() switch
            {
                "crear_analisis" => EsMedico,
                "cargar_resultados" => EsMedico,
                "validar_analisis" => EsMedico,
                "generar_informe_propio" => EsMedico,
                "generar_informe_verificado" => EsMedico || EsAsistente,
                "ver_historial_pacientes" => EsMedico || EsAsistente,
                "gestionar_usuarios" => EsAdministrador,
                "gestionar_catalogos" => EsAdministrador,
                "gestionar_backups" => EsAdministrador,
                _ => false
            };
        }
        #endregion

        #region Sobrescritura de m�todos base
        public override string ToString()
        {
            return NombreRol ?? "Rol sin nombre";
        }

        public override bool Equals(object obj)
        {
            if (obj is Rol other)
                return IdRol == other.IdRol;
            return false;
        }

        public override int GetHashCode()
        {
            return IdRol.GetHashCode();
        }
        #endregion
    }
}