namespace SALC
{
    /// <summary>
    /// Define las funcionalidades (features) del sistema SALC.
    /// Cada feature se asigna a roles espec�ficos seg�n el ERS-SALC_IEEE830.
    /// </summary>
    public enum AppFeature
    {
        Dashboard,
        
        // GESTI�N DE DATOS
        GestionPacientes,         // Cl�nicos y Asistentes (RF-03, RF-04, RF-05)
        GestionEstudios,          // Solo Cl�nicos (crear an�lisis - RF-06)
        
        // OPERACIONES DE AN�LISIS
        CargaResultados,          // Solo Cl�nicos (cargar y validar - RF-07, RF-18)
        RecepcionMuestras,        // Solo Asistentes (recepcionar muestra - RF-17)
        
        // INFORMES Y COMUNICACI�N
        GenerarInformes,          // Cl�nicos y Asistentes (imprimir validados)
        Notificaciones,           // Solo Cl�nicos (notificar pacientes - RF-16)
        HistorialOrdenes,         // Cl�nicos y Asistentes
        
        // ADMINISTRACI�N
        GestionUsuarios,          // Solo Administrador (RF-10)
        ConfigSistema,            // Solo Administrador
        CopiasSeguridad,          // Solo Administrador
        Seguridad                 // Solo Administrador
        
        // NOTA: Turnos NO existe en el sistema SALC seg�n ERS-SALC_IEEE830
        // NOTA: Agenda m�dica NO est� contemplada en el alcance del sistema
    }
}
