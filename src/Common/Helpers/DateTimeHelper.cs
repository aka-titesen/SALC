using System;

namespace SALC.Common.Helpers
{
    /// <summary>
    /// Clase helper para operaciones comunes con fechas y horas.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Formatos de fecha comunes utilizados en el sistema
        /// </summary>
        public static class Formats
        {
            public const string ShortDate = "dd/MM/yyyy";
            public const string LongDate = "dddd, dd 'de' MMMM 'de' yyyy";
            public const string DateTime = "dd/MM/yyyy HH:mm";
            public const string DateTimeWithSeconds = "dd/MM/yyyy HH:mm:ss";
            public const string Time = "HH:mm";
            public const string TimeWithSeconds = "HH:mm:ss";
        }

        /// <summary>
        /// Convierte una fecha a string con formato corto
        /// </summary>
        /// <param name="date">Fecha a convertir</param>
        /// <returns>Fecha formateada</returns>
        public static string ToShortDateString(DateTime date)
        {
            return date.ToString(Formats.ShortDate);
        }

        /// <summary>
        /// Convierte una fecha a string con formato largo
        /// </summary>
        /// <param name="date">Fecha a convertir</param>
        /// <returns>Fecha formateada</returns>
        public static string ToLongDateString(DateTime date)
        {
            return date.ToString(Formats.LongDate);
        }

        /// <summary>
        /// Convierte una fecha y hora a string
        /// </summary>
        /// <param name="dateTime">Fecha y hora a convertir</param>
        /// <param name="includeSeconds">Si incluir segundos</param>
        /// <returns>Fecha y hora formateada</returns>
        public static string ToDateTimeString(DateTime dateTime, bool includeSeconds = false)
        {
            return dateTime.ToString(includeSeconds ? Formats.DateTimeWithSeconds : Formats.DateTime);
        }

        /// <summary>
        /// Calcula la edad en años basada en la fecha de nacimiento
        /// </summary>
        /// <param name="birthDate">Fecha de nacimiento</param>
        /// <param name="referenceDate">Fecha de referencia (opcional, por defecto hoy)</param>
        /// <returns>Edad en años</returns>
        public static int CalculateAge(DateTime birthDate, DateTime? referenceDate = null)
        {
            var reference = referenceDate ?? DateTime.Today;
            var age = reference.Year - birthDate.Year;

            if (birthDate.Date > reference.AddYears(-age))
                age--;

            return age;
        }

        /// <summary>
        /// Obtiene el primer día del mes
        /// </summary>
        /// <param name="date">Fecha de referencia</param>
        /// <returns>Primer día del mes</returns>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Obtiene el último día del mes
        /// </summary>
        /// <param name="date">Fecha de referencia</param>
        /// <returns>Último día del mes</returns>
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Verifica si una fecha es un día laboral (lunes a viernes)
        /// </summary>
        /// <param name="date">Fecha a verificar</param>
        /// <returns>True si es día laboral</returns>
        public static bool IsWorkDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        /// <summary>
        /// Obtiene el próximo día laboral
        /// </summary>
        /// <param name="date">Fecha de referencia</param>
        /// <returns>Próximo día laboral</returns>
        public static DateTime GetNextWorkDay(DateTime date)
        {
            var nextDay = date.AddDays(1);
            while (!IsWorkDay(nextDay))
            {
                nextDay = nextDay.AddDays(1);
            }
            return nextDay;
        }

        /// <summary>
        /// Convierte una fecha a solo fecha (sin hora)
        /// </summary>
        /// <param name="dateTime">Fecha y hora</param>
        /// <returns>Solo la fecha</returns>
        public static DateTime ToDateOnly(DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Verifica si dos fechas son del mismo día
        /// </summary>
        /// <param name="date1">Primera fecha</param>
        /// <param name="date2">Segunda fecha</param>
        /// <returns>True si son del mismo día</returns>
        public static bool IsSameDay(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }
    }
}
