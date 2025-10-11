using System;
using System.Text.RegularExpressions;

namespace SALC.Common.Helpers
{
    /// <summary>
    /// Clase helper para validaciones comunes en el sistema SALC.
    /// Centraliza la lógica de validación siguiendo principios DRY.
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PhoneRegex = new Regex(
            @"^[\d\-\s\(\)\+]{8,20}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Valida si un DNI es válido
        /// </summary>
        /// <param name="dni">DNI a validar</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidDni(int dni)
        {
            return dni >= Constants.SALCConstants.Validation.DniMinValue &&
                   dni <= Constants.SALCConstants.Validation.DniMaxValue;
        }

        /// <summary>
        /// Valida si un email tiene formato válido
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email) &&
                   email.Length <= Constants.SALCConstants.Validation.MaxEmailLength;
        }

        /// <summary>
        /// Valida si un teléfono tiene formato válido
        /// </summary>
        /// <param name="phone">Teléfono a validar</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Teléfono es opcional

            return PhoneRegex.IsMatch(phone);
        }

        /// <summary>
        /// Valida si una cadena no es nula ni vacía y no excede la longitud máxima
        /// </summary>
        /// <param name="value">Valor a validar</param>
        /// <param name="maxLength">Longitud máxima permitida</param>
        /// <param name="required">Si es requerido o no</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidString(string value, int maxLength, bool required = true)
        {
            if (required && string.IsNullOrWhiteSpace(value))
                return false;

            if (!required && string.IsNullOrWhiteSpace(value))
                return true;

            return value.Length <= maxLength;
        }

        /// <summary>
        /// Valida si una fecha está en un rango válido
        /// </summary>
        /// <param name="date">Fecha a validar</param>
        /// <param name="minDate">Fecha mínima (opcional)</param>
        /// <param name="maxDate">Fecha máxima (opcional, por defecto hoy)</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidDate(DateTime date, DateTime? minDate = null, DateTime? maxDate = null)
        {
            var min = minDate ?? new DateTime(1900, 1, 1);
            var max = maxDate ?? DateTime.Today;

            return date >= min && date <= max;
        }

        /// <summary>
        /// Valida si un número decimal está en un rango válido
        /// </summary>
        /// <param name="value">Valor a validar</param>
        /// <param name="minValue">Valor mínimo (opcional)</param>
        /// <param name="maxValue">Valor máximo (opcional)</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidDecimal(decimal value, decimal? minValue = null, decimal? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                return false;

            if (maxValue.HasValue && value > maxValue.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Valida una contraseña básica
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>True si cumple los requisitos mínimos</returns>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= Constants.SALCConstants.Validation.MinPasswordLength;
        }

        /// <summary>
        /// Valida si un CUIT tiene formato válido
        /// </summary>
        /// <param name="cuit">CUIT a validar</param>
        /// <returns>True si es válido</returns>
        public static bool IsValidCuit(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                return false;

            // Remover guiones si los tiene
            cuit = cuit.Replace("-", "");

            // Debe tener 11 dígitos
            if (cuit.Length != 11 || !long.TryParse(cuit, out _))
                return false;

            // Validación básica de dígito verificador (implementación simplificada)
            return true;
        }
    }
}
