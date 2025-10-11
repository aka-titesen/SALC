using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa una obra social en el sistema SALC seg�n ERS v2.7
    /// Tabla: obras_sociales
    /// </summary>
    public class ObraSocial
    {
        #region Propiedades de la tabla obras_sociales
        /// <summary>
        /// ID �nico de la obra social (PK, IDENTITY)
        /// </summary>
        public int IdObraSocial { get; set; }

        /// <summary>
        /// CUIT de la obra social (�nico, 10-13 caracteres)
        /// </summary>
        public string Cuit { get; set; }

        /// <summary>
        /// Nombre de la obra social
        /// </summary>
        public string Nombre { get; set; }
        #endregion

        #region M�todos de validaci�n
        /// <summary>
        /// Valida los datos de la obra social
        /// </summary>
        public bool EsValida()
        {
            return IdObraSocial > 0 &&
                   !string.IsNullOrWhiteSpace(Cuit) &&
                   Cuit.Length >= 10 &&
                   Cuit.Length <= 13 &&
                   !string.IsNullOrWhiteSpace(Nombre);
        }

        /// <summary>
        /// Valida el formato del CUIT (debe ser num�rico)
        /// </summary>
        public bool CuitEsValido()
        {
            if (string.IsNullOrWhiteSpace(Cuit)) return false;
            
            // Remover guiones si los tiene
            string cuitLimpio = Cuit.Replace("-", "");
            
            // Verificar que sea num�rico y tenga longitud v�lida
            return long.TryParse(cuitLimpio, out _) && 
                   cuitLimpio.Length >= 10 && 
                   cuitLimpio.Length <= 13;
        }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// CUIT formateado (solo n�meros)
        /// </summary>
        public string CuitFormateado => Cuit?.Replace("-", "") ?? "";

        /// <summary>
        /// Descripci�n completa de la obra social
        /// </summary>
        public string DescripcionCompleta => $"{Nombre} (CUIT: {Cuit})";
        #endregion

        #region Sobrescritura de m�todos base
        public override string ToString()
        {
            return Nombre ?? "Obra social sin nombre";
        }

        public override bool Equals(object obj)
        {
            if (obj is ObraSocial other)
                return IdObraSocial == other.IdObraSocial;
            return false;
        }

        public override int GetHashCode()
        {
            return IdObraSocial.GetHashCode();
        }
        #endregion
    }
}