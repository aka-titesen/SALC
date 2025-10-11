using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public class AnalisisService : IAnalisisService
    {
        public Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones)
        {
            return null;
        }

        public void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null)
        {
        }

        public IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico)
        {
            yield break;
        }

        public IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente)
        {
            yield break;
        }

        public IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis)
        {
            yield break;
        }

        public void ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
        }
    }
}
