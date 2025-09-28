// Common/Colors.cs
using System.Drawing;

namespace SALC.Common
{
    /// <summary>
    /// Colores estándar para el sistema SALC
    /// </summary>
    public static class Colors
    {
        // Colores principales
        public static readonly Color Background = Color.FromArgb(248, 249, 250);
        public static readonly Color CardBackground = Color.White;
        public static readonly Color TextPrimary = Color.FromArgb(52, 58, 64);
        public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125);
        
        // Colores de acción
        public static readonly Color Primary = Color.FromArgb(0, 123, 255);
        public static readonly Color Success = Color.FromArgb(40, 167, 69);
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);
        public static readonly Color Warning = Color.FromArgb(255, 193, 7);
        public static readonly Color Info = Color.FromArgb(23, 162, 184);
        public static readonly Color Secondary = Color.FromArgb(108, 117, 125);
        
        // Colores por módulo
        public static readonly Color Patients = Color.FromArgb(23, 162, 184);
        public static readonly Color Studies = Color.FromArgb(0, 123, 255);
        public static readonly Color Results = Color.FromArgb(255, 193, 7);
        public static readonly Color Reports = Color.FromArgb(111, 66, 193);
        public static readonly Color Notifications = Color.FromArgb(253, 126, 20);
        public static readonly Color History = Color.FromArgb(40, 167, 69);
    }
}