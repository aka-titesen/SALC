namespace SALC
{
    /// <summary>
    /// Define las funcionalidades (features) del sistema SALC.
    /// Cada feature se asigna a roles espec�ficos seg�n el ERS-SALC_IEEE830.
    /// </summary>
    public enum AppFeature
    {
        // TODO: Dashboard deshabilitado temporalmente - no definido en ERS v1.0
        // Dashboard,
        
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
        
        // ADMINISTRACI�N DE USUARIOS (Solo Administrador)
        GestionUsuarios,          // ABM usuarios internos (usuario, doctor, asistente)
        GestionPacientesAdmin,    // ABM pacientes (vista administrativa completa)
        GestionDoctoresExternos,  // ABM doctores externos
        
        // ADMINISTRACI�N DE CAT�LOGOS (Solo Administrador)
        GestionTiposAnalisis,     // ABM tipo_analisis
        GestionMetricas,          // ABM metrica
        GestionObrasSociales,     // ABM obra_social
        GestionEstados,           // ABM estado y estado_usuario
        GestionRoles,             // ABM rol
        
        // CONFIGURACI�N Y SISTEMA (Solo Administrador)
        // TODO: ConfigSistema deshabilitado temporalmente - no soportado en modelo v1.0
        // ConfigSistema,            // Configuraci�n general del sistema
        CopiasSeguridad,          // Backup y restore
        // TODO: Seguridad y AuditoriaAccesos deshabilitados temporalmente - no soportados en ERS v1.0
        // Seguridad,                // Auditor�a y logs
        // AuditoriaAccesos          // Logs de accesos de usuarios
        
        // NOTA: Turnos NO existe en el sistema SALC seg�n ERS-SALC_IEEE830
        // NOTA: Agenda m�dica NO est� contemplada en el alcance del sistema
    }
}
