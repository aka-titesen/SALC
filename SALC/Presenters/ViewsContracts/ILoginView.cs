using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface ILoginView
    {
        string DniTexto { get; }
        string Contrasenia { get; }
        event EventHandler AccederClick;
        void MostrarError(string mensaje);
        void Cerrar();
    }
}
