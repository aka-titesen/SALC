namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para servicios de hash de contraseñas.
    /// Define las operaciones para generar y verificar hashes de contraseñas de forma segura.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash
        /// </summary>
        /// <param name="plainText">Contraseña en texto plano</param>
        /// <param name="hashed">Hash de contraseña a comparar</param>
        /// <returns>True si la contraseña coincide con el hash, false en caso contrario</returns>
        bool Verify(string plainText, string hashed);

        /// <summary>
        /// Genera un hash seguro de una contraseña en texto plano
        /// </summary>
        /// <param name="plainText">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        string Hash(string plainText);

        /// <summary>
        /// Determina si una cadena es texto plano o un hash
        /// </summary>
        /// <param name="passwordHash">Cadena a evaluar</param>
        /// <returns>True si es texto plano, false si es un hash</returns>
        bool IsPlainText(string passwordHash);
    }
}
