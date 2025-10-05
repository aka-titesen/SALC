namespace SALC
{
    /// <summary>
    /// Define las funcionalidades (features) del sistema SALC.
    /// Cada feature se asigna a roles específicos según el ERS-SALC_IEEE830.
    /// </summary>
    public enum AppFeature
    {
        Dashboard,
        
        // GESTIÓN DE DATOS
        GestionPacientes,         // Clínicos y Asistentes (RF-03, RF-04, RF-05)
        GestionEstudios,          // Solo Clínicos (crear análisis - RF-06)
        
        // OPERACIONES DE ANÁLISIS
        CargaResultados,          // Solo Clínicos (cargar y validar - RF-07, RF-18)
        RecepcionMuestras,        // Solo Asistentes (recepcionar muestra - RF-17)
        
        // INFORMES Y COMUNICACIÓN
        GenerarInformes,          // Clínicos y Asistentes (imprimir validados)
        Notificaciones,           // Solo Clínicos (notificar pacientes - RF-16)
        HistorialOrdenes,         // Clínicos y Asistentes
        
        // ADMINISTRACIÓN
        GestionUsuarios,          // Solo Administrador (RF-10)
        ConfigSistema,            // Solo Administrador
        CopiasSeguridad,          // Solo Administrador
        Seguridad                 // Solo Administrador
        
        // NOTA: Turnos NO existe en el sistema SALC según ERS-SALC_IEEE830
        // NOTA: Agenda médica NO está contemplada en el alcance del sistema
    }
}
