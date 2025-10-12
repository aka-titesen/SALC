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
        
        // Según scripts: 1 = "Sin verificar", 2 = "Verificado", 3 = "Anulado"
        private const int EstadoSinVerificar = 1;
        private const int EstadoVerificado = 2;
        private const int EstadoAnulado = 3;

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
            if (a.IdEstado != EstadoSinVerificar) throw new InvalidOperationException("No se pueden modificar resultados en un análisis verificado o anulado");
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

        // Método para obtener solo análisis activos (no anulados) por médico
        public IEnumerable<Analisis> ObtenerAnalisisActivosPorMedicoCarga(int dniMedico)
        {
            return _analisisRepo.ObtenerActivosPorMedicoCarga(dniMedico);
        }

        public IEnumerable<Analisis> ObtenerAnalisisPorPaciente(int dniPaciente)
        {
            return _analisisRepo.ObtenerPorPaciente(dniPaciente);
        }

        // Método para obtener solo análisis activos (no anulados) por paciente
        public IEnumerable<Analisis> ObtenerAnalisisActivosPorPaciente(int dniPaciente)
        {
            return _analisisRepo.ObtenerActivosPorPaciente(dniPaciente);
        }

        public IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis)
        {
            return _amRepo.ObtenerResultados(idAnalisis);
        }

        public void ValidarAnalisis(int idAnalisis, int dniMedicoFirma)
        {
            var a = _analisisRepo.ObtenerPorId(idAnalisis);
            if (a == null) throw new InvalidOperationException("Análisis no encontrado");
            if (a.IdEstado != EstadoSinVerificar) throw new InvalidOperationException("El análisis ya fue verificado o está anulado");
            // Regla de negocio: solo el médico que creó (dni_carga) puede firmar
            if (a.DniCarga != dniMedicoFirma)
                throw new InvalidOperationException("Solo el médico que creó el análisis puede firmarlo");
            a.IdEstado = EstadoVerificado;
            a.DniFirma = dniMedicoFirma;
            a.FechaFirma = DateTime.Now;
            _analisisRepo.Actualizar(a);
        }

        // Método para anular análisis (baja lógica)
        public void AnularAnalisis(int idAnalisis, int dniMedico)
        {
            var a = _analisisRepo.ObtenerPorId(idAnalisis);
            if (a == null) throw new InvalidOperationException("Análisis no encontrado");
            if (a.IdEstado == EstadoAnulado) throw new InvalidOperationException("El análisis ya está anulado");
            
            // Solo el médico que creó el análisis puede anularlo
            if (a.DniCarga != dniMedico)
                throw new InvalidOperationException("Solo el médico que creó el análisis puede anularlo");
                
            a.IdEstado = EstadoAnulado;
            _analisisRepo.Actualizar(a);
        }

        // Método para eliminar análisis (baja lógica) - alias del método AnularAnalisis
        public void EliminarAnalisis(int idAnalisis, int dniMedico)
        {
            AnularAnalisis(idAnalisis, dniMedico);
        }

        public Analisis ObtenerAnalisisPorId(int idAnalisis)
        {
            return _analisisRepo.ObtenerPorId(idAnalisis);
        }

        // Método para obtener todos los análisis activos
        public IEnumerable<Analisis> ObtenerAnalisisActivos()
        {
            return _analisisRepo.ObtenerActivos();
        }

        // Método para verificar si un análisis está anulado
        public bool EstaAnulado(int idAnalisis)
        {
            var analisis = _analisisRepo.ObtenerPorId(idAnalisis);
            return analisis?.IdEstado == EstadoAnulado;
        }

        // Método para verificar si un análisis puede ser modificado
        public bool PuedeSerModificado(int idAnalisis)
        {
            var analisis = _analisisRepo.ObtenerPorId(idAnalisis);
            return analisis?.IdEstado == EstadoSinVerificar;
        }
    }
}
