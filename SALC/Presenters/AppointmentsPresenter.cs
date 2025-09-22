// Presenters/AppointmentsPresenter.cs
using System;
using System.Collections.Generic;
using SALC.Views.Interfaces;
using SALC.Models;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gesti�n de turnos
    /// Maneja la l�gica de presentaci�n entre la vista y los servicios de negocio
    /// </summary>
    public class AppointmentsPresenter
    {
        private readonly IAppointmentsView _view;

        public AppointmentsPresenter(IAppointmentsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadAppointments += OnLoadAppointments;
            _view.DeleteAppointment += OnDeleteAppointment;
            _view.SaveAppointment += OnSaveAppointment;
            _view.EditAppointment += OnEditAppointment;
            _view.FilterByDate += OnFilterByDate;
            _view.SearchAppointment += OnSearchAppointment;
        }

        private void OnLoadAppointments(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implementar carga desde el servicio de datos
                // var appointments = _appointmentService.GetAllAppointments();
                // _view.Appointments = appointments;
                // _view.RefreshAppointmentList();

                // Por ahora, datos de ejemplo
                _view.Appointments = GetSampleAppointments();
                _view.RefreshAppointmentList();
                _view.ShowMessage("Turnos cargados correctamente.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar turnos: {ex.Message}");
            }
        }

        private void OnDeleteAppointment(object sender, int appointmentId)
        {
            try
            {
                // TODO: Implementar cancelaci�n a trav�s del servicio
                // _appointmentService.CancelAppointment(appointmentId);
                
                _view.ShowMessage($"Turno ID {appointmentId} cancelado correctamente.");
                OnLoadAppointments(sender, EventArgs.Empty); // Recargar lista
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cancelar turno: {ex.Message}");
            }
        }

        private void OnSaveAppointment(object sender, Turno turno)
        {
            try
            {
                if (_view.IsEditing)
                {
                    // TODO: Implementar actualizaci�n
                    // _appointmentService.UpdateAppointment(turno);
                    _view.ShowMessage("Turno actualizado correctamente.");
                }
                else
                {
                    // TODO: Implementar creaci�n
                    // _appointmentService.CreateAppointment(turno);
                    _view.ShowMessage("Turno creado correctamente.");
                }

                _view.ClearForm();
                OnLoadAppointments(sender, EventArgs.Empty); // Recargar lista
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al guardar turno: {ex.Message}");
            }
        }

        private void OnEditAppointment(object sender, int appointmentId)
        {
            try
            {
                // TODO: Cargar datos del turno espec�fico desde el servicio
                // var turno = _appointmentService.GetAppointmentById(appointmentId);
                // if (turno != null)
                // {
                //     _view.LoadAppointmentData(turno);
                // }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al cargar datos del turno: {ex.Message}");
            }
        }

        private void OnFilterByDate(object sender, DateTime date)
        {
            try
            {
                // TODO: Implementar filtro por fecha a trav�s del servicio
                // var appointments = _appointmentService.GetAppointmentsByDate(date);
                // _view.Appointments = appointments;
                // _view.RefreshAppointmentList();
                
                _view.ShowMessage($"Mostrando turnos para: {date:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error al filtrar turnos: {ex.Message}");
            }
        }

        private void OnSearchAppointment(object sender, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    OnLoadAppointments(sender, EventArgs.Empty);
                    return;
                }

                // TODO: Implementar b�squeda a trav�s del servicio
                // var appointments = _appointmentService.SearchAppointments(searchTerm);
                // _view.Appointments = appointments;
                // _view.RefreshAppointmentList();
                
                _view.ShowMessage($"B�squeda realizada para: {searchTerm}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error en la b�squeda: {ex.Message}");
            }
        }

        /// <summary>
        /// Datos de ejemplo para testing de la interfaz
        /// </summary>
        private List<Turno> GetSampleAppointments()
        {
            return new List<Turno>
            {
                new Turno
                {
                    Id = 1,
                    PacienteId = 12345678,
                    DoctorId = 11111111,
                    FechaHora = DateTime.Today.AddHours(9),
                    Estado = "Programado",
                    Motivo = "Consulta general",
                    Observaciones = "Primera consulta",
                    Paciente = new Paciente { NroDoc = 12345678, Nombre = "Ana", Apellido = "Garc�a" },
                    Doctor = new Doctor { Dni = 11111111, Nombre = "Dr. Juan", Apellido = "P�rez" }
                },
                new Turno
                {
                    Id = 2,
                    PacienteId = 87654321,
                    DoctorId = 22222222,
                    FechaHora = DateTime.Today.AddHours(14),
                    Estado = "Confirmado",
                    Motivo = "Control de rutina",
                    Observaciones = "",
                    Paciente = new Paciente { NroDoc = 87654321, Nombre = "Carlos", Apellido = "Ruiz" },
                    Doctor = new Doctor { Dni = 22222222, Nombre = "Dra. Mar�a", Apellido = "L�pez" }
                },
                new Turno
                {
                    Id = 3,
                    PacienteId = 23456789,
                    DoctorId = 11111111,
                    FechaHora = DateTime.Today.AddDays(1).AddHours(10),
                    Estado = "Programado",
                    Motivo = "Seguimiento de tratamiento",
                    Observaciones = "Paciente en tratamiento",
                    Paciente = new Paciente { NroDoc = 23456789, Nombre = "Mar�a", Apellido = "Gonz�lez" },
                    Doctor = new Doctor { Dni = 11111111, Nombre = "Dr. Juan", Apellido = "P�rez" }
                }
            };
        }
    }
}