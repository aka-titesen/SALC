/*
 * SALC - Configuración de Colores del Sistema
 * Archivo de configuración para mantener consistencia visual
 */

using System.Drawing;

namespace SALC.UI
{
    /// <summary>
    /// Clase estática que contiene todos los colores del sistema SALC
    /// Facilita el mantenimiento y la consistencia visual
    /// </summary>
    public static class SALCColors
    {
        // ============================================
        // COLORES PRINCIPALES
        // ============================================


        /// <summary>
        /// Azul principal del sistema - representa confianza y profesionalismo médico
        /// </summary>
        public static readonly Color Primary = Color.FromArgb(0, 123, 255);

        /// <summary>
        /// Gris secundario para elementos neutros
        /// </summary>
        public static readonly Color Secondary = Color.FromArgb(108, 117, 125);

        /// <summary>
        /// Fondo general del sistema - gris muy claro
        /// </summary>
        public static readonly Color Background = Color.FromArgb(248, 249, 250);

        /// <summary>
        /// Fondo de tarjetas y paneles principales
        /// </summary>
        public static readonly Color CardBackground = Color.White;

        /// <summary>
        /// Color de texto principal
        /// </summary>
        public static readonly Color TextPrimary = Color.FromArgb(52, 58, 64);

        /// <summary>
        /// Color de texto secundario
        /// </summary>
        public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125);

        // ============================================
        // COLORES DE ESTADO
        // ============================================

        /// <summary>
        /// Verde para éxito, completado, activo
        /// </summary>
        public static readonly Color Success = Color.FromArgb(40, 167, 69);

        /// <summary>
        /// Amarillo para advertencias, pendiente
        /// </summary>
        public static readonly Color Warning = Color.FromArgb(255, 193, 7);

        /// <summary>
        /// Rojo para errores, crítico, peligro
        /// </summary>
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);

        /// <summary>
        /// Azul claro para información
        /// </summary>
        public static readonly Color Info = Color.FromArgb(23, 162, 184);

        // ============================================
        // COLORES DE MÓDULOS
        // ============================================

        /// <summary>
        /// Color para el módulo de Gestión de Pacientes
        /// </summary>
        public static readonly Color Patients = Color.FromArgb(23, 162, 184);

        /// <summary>
        /// Color para el módulo de Gestión de Estudios
        /// </summary>
        public static readonly Color Studies = Color.FromArgb(0, 123, 255);

        /// <summary>
        /// Color para el módulo de Carga de Resultados
        /// </summary>
        public static readonly Color Results = Color.FromArgb(255, 193, 7);

    /// <summary>
    /// Color para el módulo de Generación de Informes
    /// </summary>
    public static readonly Color Informes = Color.FromArgb(111, 66, 193);

        /// <summary>
        /// Color para el módulo de Notificaciones
        /// </summary>
        public static readonly Color Notifications = Color.FromArgb(253, 126, 20);

        /// <summary>
        /// Color para el módulo de Historial de Órdenes
        /// </summary>
        public static readonly Color History = Color.FromArgb(40, 167, 69);

        // ============================================
        // COLORES DE LOGIN
        // ============================================

        /// <summary>
        /// Fondo de la caja de login
        /// </summary>
        public static readonly Color LoginBoxBackground = Color.FromArgb(248, 249, 250);

        /// <summary>
        /// Fondo del formulario de login
        /// </summary>
        public static readonly Color LoginBackground = Color.FromArgb(233, 236, 239);

        /// <summary>
        /// Fondo claro para paneles de filtros
        /// </summary>
        public static readonly Color BackgroundLight = Color.FromArgb(240, 242, 245);

        // ============================================
        // COLORES DE SIDEBAR
        // ============================================

        /// <summary>
        /// Fondo del sidebar de navegación
        /// </summary>
        public static readonly Color SidebarBackground = Color.FromArgb(52, 58, 64);

        /// <summary>
        /// Color de texto del sidebar (inactivo)
        /// </summary>
        public static readonly Color SidebarText = Color.FromArgb(173, 181, 189);

        /// <summary>
        /// Color de texto del sidebar (activo)
        /// </summary>
        public static readonly Color SidebarTextActive = Color.White;

        // ============================================
        // COLORES DE BORDES
        // ============================================

        /// <summary>
        /// Color de borde estándar
        /// </summary>
        public static readonly Color Border = Color.FromArgb(222, 226, 230);

        /// <summary>
        /// Color de borde para elementos con foco
        /// </summary>
        public static readonly Color BorderFocus = Color.FromArgb(0, 123, 255);

        // ============================================
        // COLORES DE HOVER/INTERACCIÓN
        // ============================================

        /// <summary>
        /// Color primario para estado hover
        /// </summary>
        public static readonly Color PrimaryHover = Color.FromArgb(0, 86, 179);

        /// <summary>
        /// Color de fondo para hover en filas de tabla
        /// </summary>
        public static readonly Color TableRowHover = Color.FromArgb(241, 243, 245);

        /// <summary>
        /// Color de fondo para encabezados de tabla
        /// </summary>
        public static readonly Color TableHeader = Color.FromArgb(233, 236, 239);

        // ============================================
        // MÉTODOS AUXILIARES
        // ============================================

        /// <summary>
        /// Obtiene un color con transparencia ajustada
        /// </summary>
        /// <param name="color">Color base</param>
        /// <param name="alpha">Valor alpha (0-255)</param>
        /// <returns>Color con transparencia aplicada</returns>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>
        /// Oscurece un color en un porcentaje determinado
        /// </summary>
        /// <param name="color">Color base</param>
        /// <param name="percentage">Porcentaje de oscurecimiento (0.0 - 1.0)</param>
        /// <returns>Color oscurecido</returns>
        public static Color Darken(Color color, float percentage)
        {
            float factor = 1.0f - percentage;
            return Color.FromArgb(
                color.A,
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor)
            );
        }

        /// <summary>
        /// Aclara un color en un porcentaje determinado
        /// </summary>
        /// <param name="color">Color base</param>
        /// <param name="percentage">Porcentaje de aclarado (0.0 - 1.0)</param>
        /// <returns>Color aclarado</returns>
        public static Color Lighten(Color color, float percentage)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R + (255 - color.R) * percentage),
                (int)(color.G + (255 - color.G) * percentage),
                (int)(color.B + (255 - color.B) * percentage)
            );
        }

        // ============================================
        // ESTADOS DE ÓRDENES
        // ============================================

        /// <summary>
        /// Obtiene el color correspondiente a un estado de orden
        /// </summary>
        /// <param name="status">Estado de la orden</param>
        /// <returns>Color correspondiente al estado</returns>
        public static Color GetStatusColor(string status)
        {
            string key = string.IsNullOrEmpty(status) ? string.Empty : status.ToLower();
            switch (key)
            {
                case "pendiente":
                    return Warning;
                case "en proceso":
                    return Info;
                case "completado":
                    return Success;
                case "entregado":
                    return Primary;
                case "cancelado":
                    return Danger;
                default:
                    return Secondary;
            }
        }

        /// <summary>
        /// Obtiene el color de texto correspondiente a un estado de orden
        /// </summary>
        /// <param name="status">Estado de la orden</param>
        /// <returns>Color de texto correspondiente al estado</returns>
        public static Color GetStatusTextColor(string status)
        {
            string key = string.IsNullOrEmpty(status) ? string.Empty : status.ToLower();
            switch (key)
            {
                case "pendiente":
                    return TextPrimary; // Texto oscuro sobre fondo amarillo
                default:
                    return Color.White; // Texto blanco sobre fondos coloridos
            }
        }
    }
}
