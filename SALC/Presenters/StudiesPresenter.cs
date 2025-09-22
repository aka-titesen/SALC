using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    public class StudiesPresenter
    {
        private readonly IStudiesView _view;

        public StudiesPresenter(IStudiesView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.CreateRequested += (s,e) => _view.ShowMessage("Estudios", "Crear orden de an�lisis");
            _view.EditRequested += (s,e) => _view.ShowMessage("Estudios", "Editar orden de an�lisis");
            _view.DeleteRequested += (s,e) => _view.ShowMessage("Estudios", "Eliminar orden de an�lisis");
            _view.SearchRequested += (s,e) => _view.ShowMessage("Estudios", $"Buscar: '{_view.SearchText}'");
            _view.CloseRequested += (s,e) => { };
        }
    }
}
