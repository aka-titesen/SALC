using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IAnalisisService
    {
        Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones);
        void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null);
        void ValidarAnalisis(int idAnalisis, int dniMedicoFirma);
        void AnularAnalisis(int idAnalisis, int dniMedico); // Baja l�gica
        void EliminarAnalisis(int idAnalisis, int dniMedico); // Alias de AnularAnalisis
        IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico);
        IEnumerable<Analisis> ObtenerAnalisisActivosPorMedicoCarga(int dniMedico); // Solo an�lisis no anulados
        IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente);
        IEnumerable<Analisis> ObtenerAnalisisActivosPorPaciente(int dniPaciente); // Solo an�lisis no anulados
        IEnumerable<Analisis> ObtenerAnalisisActivos(); // Solo an�lisis no anulados
        IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis);
        Analisis ObtenerAnalisisPorId(int idAnalisis);
        bool EstaAnulado(int idAnalisis);
        bool PuedeSerModificado(int idAnalisis);
    }
}
