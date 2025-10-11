using System;
using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class AnalisisService : IAnalisisService
    {
        private readonly AnalisisRepositorio _analisisRepo = new AnalisisRepositorio();
        private readonly AnalisisMetricaRepositorio _amRepo = new AnalisisMetricaRepositorio();
        private readonly CatalogoRepositorio _catRepo = new CatalogoRepositorio();
        // Según scripts: 1 = "Sin verificar", 2 = "Verificado"
        private const int EstadoSinVerificar = 1;
        private const int EstadoVerificado = 2;
        public Analisis CrearAnalisis(int dniPaciente, int idTipoAnalisis, int dniMedicoCarga, string observaciones)
        {
            var a = new Analisis
            {
                DniPaciente = dniPaciente,
                IdTipoAnalisis = idTipoAnalisis,
                DniCarga = dniMedicoCarga,
                IdEstado = EstadoSinVerificar,
                DniFirma = null,
                FechaCreacion = DateTime.Now,
                FechaFirma = null,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            };
            return _analisisRepo.CrearYDevolver(a);
        }

        public void CargarResultado(int idAnalisis, int idMetrica, decimal resultado, string observaciones = null)
        {
            var a = _analisisRepo.ObtenerPorId(idAnalisis);
            if (a == null) throw new InvalidOperationException("Análisis no encontrado");
            if (a.IdEstado != EstadoSinVerificar) throw new InvalidOperationException("No se pueden modificar resultados en un análisis verificado");
            _amRepo.UpsertResultado(new AnalisisMetrica
            {
                IdAnalisis = idAnalisis,
                IdMetrica = idMetrica,
                Resultado = resultado,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            });
        }

        public IEnumerable<Analisis> ObtenerAnalisisPorMedicoCarga(int dniMedico)
        {
            return _analisisRepo.ObtenerPorMedicoCarga(dniMedico);
        }

        public IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente)
        {
            return _analisisRepo.ObtenerPorPaciente(dniPaciente);
        }

        public IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis)
        {
            return _amRepo.ObtenerResultados(idAnalisis);
        }

        public void ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
            var a = _analisisRepo.ObtenerPorId(idAnalisis);
            if (a == null) throw new InvalidOperationException("Análisis no encontrado");
            if (a.IdEstado != EstadoSinVerificar) throw new InvalidOperationException("El análisis ya fue verificado");
            // Regla de negocio: solo el médico que creó (dni_carga) puede firmar
            if (a.DniCarga != dniMedicoFirma)
                throw new InvalidOperationException("Solo el médico que creó el análisis puede firmarlo");
            a.IdEstado = EstadoVerificado;
            a.DniFirma = dniMedicoFirma;
            a.FechaFirma = DateTime.Now;
            _analisisRepo.Actualizar(a);
        }

        public Analisis ObtenerAnalisisPorId(int idAnalisis)
        {
            return _analisisRepo.ObtenerPorId(idAnalisis);
        }
    }
}
