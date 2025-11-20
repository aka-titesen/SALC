using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de gestión de catálogos del sistema.
    /// Define las operaciones para administrar obras sociales, tipos de análisis, métricas y sus relaciones.
    /// </summary>
    public interface ICatalogoService
    {
        /// <summary>
        /// Obtiene todas las obras sociales del sistema (incluye inactivas)
        /// </summary>
        /// <returns>Colección de todas las obras sociales</returns>
        IEnumerable<ObraSocial> ObtenerObrasSociales();

        /// <summary>
        /// Obtiene todos los tipos de análisis del sistema (incluye inactivos)
        /// </summary>
        /// <returns>Colección de todos los tipos de análisis</returns>
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisis();

        /// <summary>
        /// Obtiene todas las métricas del sistema (incluye inactivas)
        /// </summary>
        /// <returns>Colección de todas las métricas</returns>
        IEnumerable<Metrica> ObtenerMetricas();
        
        /// <summary>
        /// Obtiene solo las obras sociales activas
        /// </summary>
        /// <returns>Colección de obras sociales activas</returns>
        IEnumerable<ObraSocial> ObtenerObrasSocialesActivas();

        /// <summary>
        /// Obtiene solo los tipos de análisis activos
        /// </summary>
        /// <returns>Colección de tipos de análisis activos</returns>
        IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos();

        /// <summary>
        /// Obtiene solo las métricas activas
        /// </summary>
        /// <returns>Colección de métricas activas</returns>
        IEnumerable<Metrica> ObtenerMetricasActivas();
        
        /// <summary>
        /// Crea una nueva obra social
        /// </summary>
        /// <param name="os">Obra social a crear</param>
        void CrearObraSocial(ObraSocial os);

        /// <summary>
        /// Actualiza una obra social existente
        /// </summary>
        /// <param name="os">Obra social con datos actualizados</param>
        void ActualizarObraSocial(ObraSocial os);

        /// <summary>
        /// Elimina una obra social mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador de la obra social</param>
        void EliminarObraSocial(int id);

        /// <summary>
        /// Crea un nuevo tipo de análisis
        /// </summary>
        /// <param name="ta">Tipo de análisis a crear</param>
        void CrearTipoAnalisis(TipoAnalisis ta);

        /// <summary>
        /// Actualiza un tipo de análisis existente
        /// </summary>
        /// <param name="ta">Tipo de análisis con datos actualizados</param>
        void ActualizarTipoAnalisis(TipoAnalisis ta);

        /// <summary>
        /// Elimina un tipo de análisis mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador del tipo de análisis</param>
        void EliminarTipoAnalisis(int id);

        /// <summary>
        /// Crea una nueva métrica
        /// </summary>
        /// <param name="m">Métrica a crear</param>
        void CrearMetrica(Metrica m);

        /// <summary>
        /// Actualiza una métrica existente
        /// </summary>
        /// <param name="m">Métrica con datos actualizados</param>
        void ActualizarMetrica(Metrica m);

        /// <summary>
        /// Elimina una métrica mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador de la métrica</param>
        void EliminarMetrica(int id);

        /// <summary>
        /// Obtiene todas las relaciones entre tipos de análisis y métricas
        /// </summary>
        /// <returns>Colección de relaciones tipo análisis-métrica</returns>
        IEnumerable<TipoAnalisisMetrica> ObtenerRelacionesTipoAnalisisMetricas();

        /// <summary>
        /// Obtiene las métricas asociadas a un tipo de análisis específico
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <returns>Colección de métricas del tipo de análisis</returns>
        IEnumerable<Metrica> ObtenerMetricasPorTipoAnalisis(int idTipoAnalisis);

        /// <summary>
        /// Crea una nueva relación entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        void CrearRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);

        /// <summary>
        /// Elimina una relación entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        void EliminarRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);

        /// <summary>
        /// Actualiza todas las relaciones de un tipo de análisis con métricas
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idsMetricas">Colección de identificadores de métricas a asociar</param>
        void ActualizarRelacionesTipoAnalisis(int idTipoAnalisis, IEnumerable<int> idsMetricas);

        /// <summary>
        /// Verifica si existe una relación entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        /// <returns>True si existe la relación, false en caso contrario</returns>
        bool ExisteRelacionTipoAnalisisMetrica(int idTipoAnalisis, int idMetrica);
    }
}
