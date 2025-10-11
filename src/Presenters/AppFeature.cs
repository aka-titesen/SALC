namespace SALC
{
    /// <summary>
    /// Define las funcionalidades (features) del sistema SALC.
    /// Cada feature se asigna a roles específicos según el ERS-SALC_IEEE830.
    /// </summary>
    public enum AppFeature
    {
        // TODO: Dashboard deshabilitado temporalmente - no definido en ERS v1.0
        // Dashboard,
        
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
        
        // ADMINISTRACIÓN DE USUARIOS (Solo Administrador)
        GestionUsuarios,          // ABM usuarios internos (usuario, doctor, asistente)
        GestionPacientesAdmin,    // ABM pacientes (vista administrativa completa)
        GestionDoctoresExternos,  // ABM doctores externos
        
        // ADMINISTRACIÓN DE CATÁLOGOS (Solo Administrador)
        GestionTiposAnalisis,     // ABM tipo_analisis
        GestionMetricas,          // ABM metrica
        GestionObrasSociales,     // ABM obra_social
        GestionEstados,           // ABM estado y estado_usuario
        GestionRoles,             // ABM rol
        
        // CONFIGURACIÓN Y SISTEMA (Solo Administrador)
        // TODO: ConfigSistema deshabilitado temporalmente - no soportado en modelo v1.0
        // ConfigSistema,            // Configuración general del sistema
        CopiasSeguridad,          // Backup y restore
        // TODO: Seguridad y AuditoriaAccesos deshabilitados temporalmente - no soportados en ERS v1.0
        // Seguridad,                // Auditoría y logs
        // AuditoriaAccesos          // Logs de accesos de usuarios
        
        // NOTA: Turnos NO existe en el sistema SALC según ERS-SALC_IEEE830
        // NOTA: Agenda médica NO está contemplada en el alcance del sistema
    }
}
