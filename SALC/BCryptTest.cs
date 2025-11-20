using System;
using System.Windows.Forms;
using SALC.BLL;

namespace SALC
{
    /// <summary>
    /// Utilidad de prueba para verificar el correcto funcionamiento del algoritmo BCrypt.
    /// Valida que las contraseñas de prueba puedan ser verificadas correctamente con sus hashes.
    /// </summary>
    public class BCryptTest
    {
        /// <summary>
        /// Ejecuta pruebas de verificación de BCrypt con casos de prueba predefinidos.
        /// Verifica que las contraseñas de los usuarios de prueba coincidan con sus hashes.
        /// </summary>
        public static void TestBCrypt()
        {
            var hasher = new DefaultPasswordHasher();

            // Contraseñas del script de inicialización para testing
            var testCases = new[]
            {
                new { Password = "9ffe8/5<7X1}", Hash = "$2a$12$URP1nbn2iSYn5/cEFwcaMeN8N.8SR1TaL3FMwFvYthH6c7DAfxWWm", User = "Juan Pérez (Admin)" },
                new { Password = "5kA5GM^087C`", Hash = "$2a$12$xR2X4M31NjPdlYXefWGhVO/mhEY4wEcHGtrIT09zPWFyjcH49k7.q", User = "María González (Médico)" },
                new { Password = "0Z1AiD'-65-*", Hash = "$2a$12$o7V3ylw3QSIO2qk2QagJKubqaJFVoomZv3lTo3mVjeUuCTz6xTUsO", User = "Sofía Gundisalvo (Médico)" },
                new { Password = "&8A2Ve9&#Xmi", Hash = "$2a$12$y98vRL/bjbV.U2.DPSpciOLaqSBPNxQk.0uY.fBe.FZDdP3KVKm8a", User = "Carlos Ramírez (Asistente)" },
                new { Password = "GGeI((h71485", Hash = "$2a$12$PTg6.zjtrOe1NcGegfAl/elM95qtmTOEmHrAetH39m7lQw5wXKq9.", User = "Erika Miralles (Asistente)" }
            };

            string results = "=== PRUEBA BCrypt ===\n\n";
            bool allPassed = true;

            foreach (var testCase in testCases)
            {
                try
                {
                    bool result = hasher.Verify(testCase.Password, testCase.Hash);
                    results += $"- {testCase.User}\n";
                    results += $"  Contraseña: '{testCase.Password}'\n";
                    results += $"  Hash: {testCase.Hash.Substring(0, 20)}...\n";
                    results += $"  Resultado: {(result ? "PASS" : "FAIL")}\n\n";
                    
                    if (!result) allPassed = false;
                }
                catch (Exception ex)
                {
                    results += $"- {testCase.User}\n";
                    results += $"  ERROR: {ex.Message}\n\n";
                    allPassed = false;
                }
            }

            results += $"\n=== RESUMEN ===\n";
            results += $"Estado general: {(allPassed ? "TODAS LAS PRUEBAS PASARON" : "ALGUNAS PRUEBAS FALLARON")}\n";

            // Mostrar resultado en ventana y debug output
            MessageBox.Show(results, "Prueba BCrypt", MessageBoxButtons.OK, 
                allPassed ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            System.Diagnostics.Debug.WriteLine(results);
        }
    }
}