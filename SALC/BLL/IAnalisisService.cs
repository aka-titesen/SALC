using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IAnalisisService
    {
        Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones);
        void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null);
        void ValidarAnalisis(int idAnalisis, int dniMedicoFirma);
        IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico);
        IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente);
        IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis);
    }
}
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IAnalisisService
    {
        Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones);
        void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null);
        void ValidarAnalisis(int idAnalisis, int dniMedicoFirma);
        IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico);
        IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente);
        IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis);
    }
}
