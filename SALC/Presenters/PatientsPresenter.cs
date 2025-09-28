using System;
using System.Collections.Generic;
using SALC.Views.Interfaces;
using SALC.Models;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de pacientes
    /// Maneja la lógica de presentación entre la vista y los servicios de negocio
    /// </summary>
    public class PatientsPresenter
    {
        private readonly IPatientsView _view;

        public PatientsPresenter(IPatientsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadPatients += OnLoadPatients;
            _view.DeletePatient += OnDeletePatient;
            _view.SavePatient += OnSavePatient;
            _view.EditPatient += OnEditPatient;
            _view.SearchPatient += OnSearchPatient;
            _view.ViewPatientHistory += OnViewPatientHistory;
        }

        private void OnLoadPatients(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implementar carga desde el servicio de datos
                // var patients = _patientService.GetAllPatients();
                // _view.Patients = patients;
                // _view.RefreshPatientList();

                // Por ahora, datos de ejemplo
                _view.Patients = GetSamplePatients();
                _view.RefreshPatientList();
                _view.ShowMessage("Pacientes cargados correctamente.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar pacientes: {ex.Message}");
            }
        }

        private void OnDeletePatient(object sender, int nroDoc)
        {
            try
            {
                // TODO: Implementar eliminación a través del servicio
                // _patientService.DeletePatient(nroDoc);
                
                _view.ShowMessage($"Paciente con documento {nroDoc} eliminado correctamente.");
                OnLoadPatients(sender, EventArgs.Empty); // Recargar lista
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al eliminar paciente: {ex.Message}");
            }
        }

        private void OnSavePatient(object sender, Paciente paciente)
        {
            try
            {
                if (_view.IsEditing)
                {
                    // TODO: Implementar actualización
                    // _patientService.UpdatePatient(paciente);
                    _view.ShowMessage("Paciente actualizado correctamente.");
                }
                else
                {
                    // TODO: Implementar creación
                    // _patientService.CreatePatient(paciente);
                    _view.ShowMessage("Paciente creado correctamente.");
                }

                _view.ClearForm();
                OnLoadPatients(sender, EventArgs.Empty); // Recargar lista
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al guardar paciente: {ex.Message}");
            }
        }

        private void OnEditPatient(object sender, int nroDoc)
        {
            try
            {
                // TODO: Cargar datos del paciente específico desde el servicio
                // var paciente = _patientService.GetPatientByDocument(nroDoc);
                // if (paciente != null)
                // {
                //     _view.LoadPatientData(paciente);
                // }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar datos del paciente: {ex.Message}");
            }
        }

        private void OnSearchPatient(object sender, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    OnLoadPatients(sender, EventArgs.Empty);
                    return;
                }

                // TODO: Implementar búsqueda a través del servicio
                // var patients = _patientService.SearchPatients(searchTerm);
                // _view.Patients = patients;
                // _view.RefreshPatientList();
                
                _view.ShowMessage($"Búsqueda realizada para: {searchTerm}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error en la búsqueda: {ex.Message}");
            }
        }

        private void OnViewPatientHistory(object sender, int nroDoc)
        {
            try
            {
                // TODO: Abrir ventana de historial del paciente
                // var historyForm = new PatientHistoryForm(nroDoc);
                // historyForm.ShowDialog();
                
                _view.ShowMessage($"Mostrando historial del paciente {nroDoc}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al mostrar historial: {ex.Message}");
            }
        }

        /// <summary>
        /// Datos de ejemplo para testing de la interfaz
        /// </summary>
        private List<Paciente> GetSamplePatients()
        {
            return new List<Paciente>
            {
                new Paciente
                {
                    NroDoc = 12345678,
                    TipoDoc = "DNI",
                    Nombre = "Ana",
                    Apellido = "García",
                    Sexo = "F",
                    FechaNacimiento = new DateTime(1985, 5, 15),
                    Direccion = "Av. Corrientes 1234",
                    Localidad = "CABA",
                    Provincia = "Buenos Aires",
                    Telefono = "11-4567-8901",
                    Mail = "ana.garcia@email.com",
                    IdObraSocial = 1,
                    ObraSocial = new ObraSocial { IdObraSocial = 1, Nombre = "OSDE" }
                },
                new Paciente
                {
                    NroDoc = 87654321,
                    TipoDoc = "DNI",
                    Nombre = "Carlos",
                    Apellido = "Ruiz",
                    Sexo = "M",
                    FechaNacimiento = new DateTime(1978, 12, 3),
                    Direccion = "San Martín 567",
                    Localidad = "Rosario",
                    Provincia = "Santa Fe",
                    Telefono = "341-234-5678",
                    Mail = "carlos.ruiz@email.com",
                    IdObraSocial = 2,
                    ObraSocial = new ObraSocial { IdObraSocial = 2, Nombre = "Swiss Medical" }
                },
                new Paciente
                {
                    NroDoc = 23456789,
                    TipoDoc = "DNI",
                    Nombre = "María",
                    Apellido = "López",
                    Sexo = "F",
                    FechaNacimiento = new DateTime(1992, 8, 22),
                    Direccion = "Belgrano 890",
                    Localidad = "Córdoba",
                    Provincia = "Córdoba",
                    Telefono = "351-567-8901",
                    Mail = "maria.lopez@email.com",
                    IdObraSocial = 1,
                    ObraSocial = new ObraSocial { IdObraSocial = 1, Nombre = "OSDE" }
                }
            };
        }
    }
}
