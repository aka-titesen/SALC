using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface IPanelAsistenteView
    {
        // Historial pacientes
        event EventHandler BuscarHistorialClick;

        // Generar informe verificado
        event EventHandler GenerarInformeClick;
    }
}
