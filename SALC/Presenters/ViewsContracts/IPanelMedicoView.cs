using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelMedicoView
    {
        // Crear análisis
        event EventHandler CrearAnalisisClick;

        // Cargar resultados
        event EventHandler CargarResultadosGuardarClick;

        // Validar/Firmar
        event EventHandler FirmarAnalisisClick;

        // Generar informe (stub)
        event EventHandler GenerarInformeClick;
    }
}
