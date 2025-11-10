import random
import string
from datetime import datetime, timedelta, date
from dateutil.relativedelta import relativedelta
import pyodbc
from faker import Faker
import decimal

# =========================
# CONFIGURACIÓN
# =========================
SERVER   = "localhost\\SQLEXPRESS"  # o "localhost" o tu instancia
DATABASE = "SALC"
UID      = ""
PWD      = ""
TRUSTED  = True  # si usás autenticación integrada -> True y deja UID/PWD en blanco

# Tamaños de datos
N_USUARIOS          = 150       # Total. Se divide entre admin, médicos y asistentes
PCT_MEDICOS         = 0.30      # 30% del total de usuarios serán médicos (45)
PCT_ASISTENTES      = 0.40      # 40% del total de usuarios serán asistentes (60)
N_OBRAS_SOCIALES    = 20
N_PACIENTES         = 600
ANALISIS_X_PACIENTE = (2, 8)    # min, max de análisis por paciente

# Semilla reproducible
RANDOM_SEED = 42
random.seed(RANDOM_SEED)
fake = Faker("es_AR")
Faker.seed(RANDOM_SEED)

# Contraseña en texto plano (la app la hashea al loguear)
DEFAULT_PASSWORD = "salc123"

# =========================
# DEFINICIONES REALISTAS (CORE)
# =========================

# (Nombre: (Unidad, MinRef, MaxRef))
METRICAS_PRESETS = {
    # Hemograma
    'Recuento Globulos Rojos': ('millones/µL', 4.2, 5.9),
    'Recuento Globulos Blancos': ('miles/µL', 4.5, 11.0),
    'Hemoglobina': ('g/dL', 13.5, 17.5),
    'Hematocrito': ('%', 41.0, 53.0),
    'Plaquetas': ('miles/µL', 150.0, 450.0),
    'VCM': ('fL', 80.0, 100.0),
    'HCM': ('pg', 27.0, 34.0),
    
    # Lipídico
    'Colesterol Total': ('mg/dL', 120.0, 200.0),
    'Colesterol HDL': ('mg/dL', 40.0, 60.0),
    'Colesterol LDL': ('mg/dL', 70.0, 130.0),
    'Trigliceridos': ('mg/dL', 50.0, 150.0),
    
    # Glucémico
    'Glucosa en Ayunas': ('mg/dL', 70.0, 100.0),
    'Hemoglobina Glicosilada (HbA1c)': ('%', 4.0, 5.6),
    
    # Renal
    'Urea': ('mg/dL', 10.0, 50.0),
    'Creatinina': ('mg/dL', 0.6, 1.2),
    'Acido Urico': ('mg/dL', 2.5, 7.0),
    'Filtrado Glomerular (eGFR)': ('mL/min/1.73m²', 90.0, 120.0),

    # Hepático
    'TGO (AST)': ('U/L', 5, 40),
    'TGP (ALT)': ('U/L', 7, 56),
    'Bilirrubina Total': ('mg/dL', 0.2, 1.2),
    'Fosfatasa Alcalina (FAL)': ('U/L', 44, 147),
    
    # Tiroideo
    'TSH': ('µUI/mL', 0.4, 4.0),
    'T4 Libre': ('ng/dL', 0.8, 1.8),
    'T3 Total': ('ng/dL', 80, 200),
    
    # Ionograma
    'Sodio (Na)': ('mEq/L', 135, 145),
    'Potasio (K)': ('mEq/L', 3.5, 5.0),
    'Cloro (Cl)': ('mEq/L', 98, 107)
}

# Mapa de relaciones
ANALISIS_METRICA_MAP = {
    'Hemograma Completo': [
        'Recuento Globulos Rojos', 'Recuento Globulos Blancos', 'Hemoglobina', 
        'Hematocrito', 'Plaquetas', 'VCM', 'HCM'
    ],
    'Perfil Lipidico': [
        'Colesterol Total', 'Colesterol HDL', 'Colesterol LDL', 'Trigliceridos'
    ],
    'Perfil Glucemico': [
        'Glucosa en Ayunas', 'Hemoglobina Glicosilada (HbA1c)'
    ],
    'Funcion Renal (Uremia)': [
        'Urea', 'Creatinina', 'Acido Urico', 'Filtrado Glomerular (eGFR)'
    ],
    'Hepatograma Basico': [
        'TGO (AST)', 'TGP (ALT)', 'Bilirrubina Total', 'Fosfatasa Alcalina (FAL)'
    ],
    'Perfil Tiroideo Basico': [
        'TSH', 'T4 Libre'
    ],
    'Ionograma Plasmatico': [
        'Sodio (Na)', 'Potasio (K)', 'Cloro (Cl)'
    ]
}

ESPECIALIDADES = [
    "Bioquimica Clinica", "Hematologia", "Endocrinologia", "Nefrologia",
    "Cardiologia", "Medicina Interna", "Laboratorio General"
]


# =========================
# CONEXIÓN
# =========================
def get_connection():
    if TRUSTED:
        conn_str = (
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;"
        )
    else:
        conn_str = (
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={SERVER};DATABASE={DATABASE};UID={UID};PWD={PWD};"
        )
    # IMPORTANTE: autocommit=False para controlar la transacción
    return pyodbc.connect(conn_str, autocommit=False)

# =========================
# HELPERS
# =========================

def clear_data(cur):
    """Limpia todas las tablas en el orden correcto para evitar errores de FK."""
    print("  ... Limpiando datos existentes (orden FK)...")
    
    tablas_con_identity = []
    
    # 1. Tablas transaccionales (las que apuntan a todo)
    cur.execute("IF OBJECT_ID('dbo.analisis_metrica', 'U') IS NOT NULL DELETE FROM dbo.analisis_metrica;")
    cur.execute("IF OBJECT_ID('dbo.analisis', 'U') IS NOT NULL DELETE FROM dbo.analisis;")
    tablas_con_identity.append('analisis')

    # 2. Tabla de enlace N:N
    cur.execute("IF OBJECT_ID('dbo.tipo_analisis_metrica', 'U') IS NOT NULL DELETE FROM dbo.tipo_analisis_metrica;")

    # 3. Pacientes (apunta a Obras Sociales)
    cur.execute("IF OBJECT_ID('dbo.pacientes', 'U') IS NOT NULL DELETE FROM dbo.pacientes;")

    # 4. Extensiones de Usuario (apuntan a Usuarios y Médicos)
    cur.execute("IF OBJECT_ID('dbo.asistentes', 'U') IS NOT NULL DELETE FROM dbo.asistentes;")
    cur.execute("IF OBJECT_ID('dbo.medicos', 'U') IS NOT NULL DELETE FROM dbo.medicos;")
    
    # 5. Backups (apunta a Usuarios)
    cur.execute("IF OBJECT_ID('dbo.historial_backup', 'U') IS NOT NULL DELETE FROM dbo.historial_backup;")
    tablas_con_identity.append('historial_backup')

    # 6. Usuarios (apunta a Roles)
    cur.execute("IF OBJECT_ID('dbo.usuarios', 'U') IS NOT NULL DELETE FROM dbo.usuarios;")

    # 7. Catálogos (las tablas base)
    cur.execute("IF OBJECT_ID('dbo.roles', 'U') IS NOT NULL DELETE FROM dbo.roles;")
    tablas_con_identity.append('roles')
    
    cur.execute("IF OBJECT_ID('dbo.estados_analisis', 'U') IS NOT NULL DELETE FROM dbo.estados_analisis;")
    tablas_con_identity.append('estados_analisis')
    
    cur.execute("IF OBJECT_ID('dbo.obras_sociales', 'U') IS NOT NULL DELETE FROM dbo.obras_sociales;")
    tablas_con_identity.append('obras_sociales')
    
    cur.execute("IF OBJECT_ID('dbo.tipos_analisis', 'U') IS NOT NULL DELETE FROM dbo.tipos_analisis;")
    tablas_con_identity.append('tipos_analisis')
    
    cur.execute("IF OBJECT_ID('dbo.metricas', 'U') IS NOT NULL DELETE FROM dbo.metricas;")
    tablas_con_identity.append('metricas')

    # 8. Resetear contadores IDENTITY
    for tabla in tablas_con_identity:
        cur.execute(f"IF OBJECT_ID('dbo.{tabla}', 'U') IS NOT NULL DBCC CHECKIDENT ('dbo.{tabla}', RESEED, 0);")
    
    print("  ... Limpieza completada.")


def unique_email(base, idx, dnies_sesion):
    email = f"{base}.{idx}@example.com".replace(" ", "").lower()
    while email in dnies_sesion:
        email = f"{base}.{idx}{random.randint(10,99)}@example.com".replace(" ", "").lower()
    dnies_sesion.add(email)
    return email

def rand_dni(existing):
    while True:
        dni = random.randint(15_000_000, 45_999_999)
        if dni not in existing:
            existing.add(dni)
            return dni

def rand_cuit_real(existing):
    while True:
        prefijo = random.choice(['20', '23', '24', '27', '30'])
        base = random.randint(15_000_000, 45_999_999)
        dv = random.randint(0, 9)
        cuit = f"{prefijo}-{base}-{dv}"
        if cuit not in existing:
            existing.add(cuit)
            return cuit

def rand_phone():
    # Formato CABA/GBA o Interior
    if random.random() < 0.5:
        # CABA/GBA: 11-XXXX-XXXX
        return f"+54 9 11 {random.randint(3000, 6999)}-{random.randint(1000, 9999)}"
    else:
        # Interior: 3XX-XXXXXX
        pref = random.choice(['221', '223', '261', '264', '341', '342', '343', '351', '379', '381', '387'])
        return f"+54 9 {pref} {random.randint(400000, 699999)}"

def dt_between(days_back_start, days_back_end):
    days = random.randint(days_back_end, days_back_start)
    return datetime.now() - timedelta(days=days)

def clamp(x, lo, hi):
    return max(lo, min(hi, x))

# =========================
# INSERTORES (CORREGIDOS V7)
# =========================

def seed_roles(cur):
    # "Medico" sin acento
    roles = ["Administrador", "Medico", "Asistente"]
    for r in roles:
        cur.execute("INSERT INTO dbo.roles(nombre_rol) VALUES (?)", r)
    
    cur.execute("SELECT id_rol, nombre_rol FROM dbo.roles")
    # Devuelve el mapa, pero la función de usuarios ya no lo usará
    return {row.nombre_rol: row.id_rol for row in cur.fetchall()}


def seed_estados_analisis(cur):
    estados = ["Sin verificar", "Verificado"]
    for e in estados:
        cur.execute("INSERT INTO dbo.estados_analisis(descripcion) VALUES (?)", e)
        
    cur.execute("SELECT id_estado, descripcion FROM dbo.estados_analisis")
    return {row.descripcion: row.id_estado for row in cur.fetchall()}


def seed_usuarios_y_extensiones(cur, roles_map):
    
    dnies_sesion = set()
    emails_sesion = set()
    
    usuarios_rows = []
    medicos_rows = []
    asistentes_rows = []
    
    # 1. Definir cantidades
    n_medicos = int(N_USUARIOS * PCT_MEDICOS)
    n_asistentes = int(N_USUARIOS * PCT_ASISTENTES)
    n_admins = max(1, N_USUARIOS - n_medicos - n_asistentes) # Al menos 1 admin
    
    # --- Lógica V6 (Hardcodeada) ---
    # 1 = Administrador
    # 2 = Medico
    # 3 = Asistente
    id_rol_admin = 1
    id_rol_medico = 2
    id_rol_asistente = 3

    print(f"  ... Generando {n_admins} Admins (Rol {id_rol_admin}), {n_medicos} Medicos (Rol {id_rol_medico}), {n_asistentes} Asistentes (Rol {id_rol_asistente})...")

    # 2. Generar Admins
    for i in range(n_admins):
        dni = rand_dni(dnies_sesion)
        nombre = fake.first_name()
        apellido = fake.last_name()
        email = unique_email(f"admin.{apellido}".lower(), i, emails_sesion)
        estado = "Activo"
        usuarios_rows.append((dni, nombre, apellido, email, DEFAULT_PASSWORD, id_rol_admin, estado))

    # 3. Generar Médicos
    dnies_medicos = []
    for i in range(n_medicos):
        dni = rand_dni(dnies_sesion)
        dnies_medicos.append(dni)
        nombre = fake.first_name()
        apellido = fake.last_name()
        email = unique_email(f"{nombre[0].lower()}{apellido.lower()}", i, emails_sesion)
        estado = "Activo" if random.random() < 0.95 else "Inactivo"
        # Se usa id_rol_medico (2)
        usuarios_rows.append((dni, nombre, apellido, email, DEFAULT_PASSWORD, id_rol_medico, estado))
        
        # Extensión Médico
        matricula = random.randint(10_000, 9_999_999) # Asumimos única por DNI
        especialidad = random.choice(ESPECIALIDADES)
        medicos_rows.append((dni, matricula, especialidad))

    # 4. Generar Asistentes
    if not dnies_medicos:
        print("  ... ADVERTENCIA: No se pueden crear asistentes si no hay médicos (supervisores).")
        # Fallback por si n_medicos = 0
        if id_rol_admin == id_rol_medico: 
             print("  ... ERROR CRITICO: No hay médicos para supervisar. Saltando asistentes.")
             n_asistentes = 0
        else:
             dnies_medicos = [u[0] for u in usuarios_rows if u[5] == id_rol_medico]
             if not dnies_medicos:
                  print("  ... ERROR CRITICO: No hay médicos para supervisar. Saltando asistentes.")
                  n_asistentes = 0

    if dnies_medicos: # Solo crear asistentes si hay medicos
        for i in range(n_asistentes):
            dni = rand_dni(dnies_sesion)
            nombre = fake.first_name()
            apellido = fake.last_name()
            email = unique_email(f"{nombre.lower()}.{apellido.lower()}", i, emails_sesion)
            estado = "Activo" if random.random() < 0.98 else "Inactivo"
            usuarios_rows.append((dni, nombre, apellido, email, DEFAULT_PASSWORD, id_rol_asistente, estado))
            
            # Extensión Asistente
            dni_supervisor = random.choice(dnies_medicos)
            fecha_ing = date.today() - relativedelta(months=random.randint(1, 48))
            asistentes_rows.append((dni, dni_supervisor, fecha_ing))
    else:
        print("  ... Se salteó la creación de asistentes.")


    # 5. Insertar en BD
    cur.executemany("""
        INSERT INTO dbo.usuarios(dni, nombre, apellido, email, password_hash, id_rol, estado)
        VALUES (?, ?, ?, ?, ?, ?, ?)
    """, usuarios_rows)
    
    cur.executemany("""
        INSERT INTO dbo.medicos(dni, nro_matricula, especialidad) VALUES (?, ?, ?)
    """, medicos_rows)
    
    cur.executemany("""
        INSERT INTO dbo.asistentes(dni, dni_supervisor, fecha_ingreso) VALUES (?, ?, ?)
    """, asistentes_rows)

    # Retornar listas de DNIs para FKs
    all_dnies = [u[0] for u in usuarios_rows]
    medicos_dnies_list = [m[0] for m in medicos_rows] 
    
    return all_dnies, medicos_dnies_list


def seed_obras_sociales(cur):
    cuit_existentes = set()
    rows = []
    for _ in range(N_OBRAS_SOCIALES):
        cuit = rand_cuit_real(cuit_existentes)
        nombre = fake.company()
        estado = "Activo" if random.random() < 0.9 else "Inactivo"
        rows.append((cuit, nombre, estado))
        
    cur.executemany("INSERT INTO dbo.obras_sociales(cuit, nombre, estado) VALUES (?, ?, ?)", rows)
    
    cur.execute("SELECT id_obra_social FROM dbo.obras_sociales")
    return [r.id_obra_social for r in cur.fetchall()]


def seed_pacientes(cur, obras_ids, dnies_usuarios):
    dnies = set(dnies_usuarios) # Pacientes NO pueden tener DNI de usuario
    rows = []
    for i in range(N_PACIENTES):
        dni = rand_dni(dnies)
        nombre = fake.first_name()
        apellido = fake.last_name()
        
        years_back = random.randint(1, 95)
        fecha_nac = date.today() - relativedelta(years=years_back, months=random.randint(0, 11), days=random.randint(0, 27))
        
        sexo = random.choice(["M", "F", "X"])
        email = None if random.random() < 0.35 else f"{nombre.lower()}.{apellido.lower()}{i}@pacientes.com"
        tel = None if random.random() < 0.25 else rand_phone()
        id_obra = None if not obras_ids or random.random() < 0.30 else random.choice(obras_ids)
        estado = "Activo" if random.random() < 0.93 else "Inactivo"
        rows.append((dni, nombre, apellido, fecha_nac, sexo, email, tel, id_obra, estado))
        
    cur.executemany("""
        INSERT INTO dbo.pacientes(dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social, estado)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, rows)
    
    cur.execute("SELECT dni FROM dbo.pacientes")
    return [r.dni for r in cur.fetchall()]


def seed_metricas(cur):
    rows = []
    for nombre, (unidad, vmin, vmax) in METRICAS_PRESETS.items():
        estado = "Activo"
        rows.append((nombre, unidad, vmax, vmin, estado))
        
    cur.executemany("""
        INSERT INTO dbo.metricas(nombre, unidad_medida, valor_maximo, valor_minimo, estado)
        VALUES (?, ?, ?, ?, ?)
    """, rows)
    
    cur.execute("SELECT id_metrica, nombre FROM dbo.metricas")
    return {row.nombre: row.id_metrica for row in cur.fetchall()}


def seed_tipos_analisis(cur):
    rows = [(desc, "Activo") for desc in ANALISIS_METRICA_MAP.keys()]
    
    cur.executemany("INSERT INTO dbo.tipos_analisis(descripcion, estado) VALUES (?, ?)", rows)
    
    cur.execute("SELECT id_tipo_analisis, descripcion FROM dbo.tipos_analisis")
    return {row.descripcion: row.id_tipo_analisis for row in cur.fetchall()}


def seed_tam(cur, tipos_map, metricas_map):
    """Puebla la tabla 'tipo_analisis_metrica' usando el mapa realista."""
    rows = []
    for tipo_desc, metricas_nombres in ANALISIS_METRICA_MAP.items():
        id_tipo = tipos_map.get(tipo_desc)
        if not id_tipo:
            continue
        
        for metrica_nombre in metricas_nombres:
            id_metrica = metricas_map.get(metrica_nombre)
            if id_metrica:
                rows.append((id_tipo, id_metrica))

    if not rows:
        print("  ... ADVERTENCIA: No se generaron relaciones Tipo-Métrica. Revisa los mapas.")
        return {}

    cur.executemany("""
        INSERT INTO dbo.tipo_analisis_metrica(id_tipo_analisis, id_metrica)
        VALUES (?, ?)
    """, rows)

    # Devolver el mapa pero con IDs, para el seeder de análisis
    mapa_ids = {}
    for id_tipo, id_metrica in rows:
        mapa_ids.setdefault(id_tipo, []).append(id_metrica)
    return mapa_ids


def seed_analisis_y_metricas(cur, pacientes_dnies, medicos_dnies, tipos_map_desc, estados_map, tipo_metrica_map_ids, metricas_map_nombre):
    """
    Función con lógica V4 (dni_firma == dni_carga).
    """
    if not medicos_dnies or not pacientes_dnies or not tipo_metrica_map_ids:
        print("  ... ADVERTENCIA: Faltan datos maestros (médicos, pacientes o mapa T-M). Saltando 'analisis' y 'analisis_metrica'.")
        return
    
    if not medicos_dnies:
         print("  ... ADVERTENCIA: No hay médicos en la lista, no se pueden crear análisis.")
         return

    # Pre-cargar rangos de metricas para generar resultados
    cur.execute("SELECT id_metrica, valor_minimo, valor_maximo FROM dbo.metricas")
    rangos_metricas = {
        r.id_metrica: (
            decimal.Decimal(r.valor_minimo or 0), 
            decimal.Decimal(r.valor_maximo or 100)
        ) 
        for r in cur.fetchall()
    }

    analisis_rows = []
    tipos_ids_lista = list(tipos_map_desc.values()) # IDs de los tipos de análisis
    
    for dni_p in pacientes_dnies:
        cant = random.randint(*ANALISIS_X_PACIENTE)
        for _ in range(cant):
            id_tipo = random.choice(tipos_ids_lista)
            
            # Foco en 'Sin verificar' y 'Verificado'
            estado_desc = random.choice(["Sin verificar", "Verificado"])
            id_estado = estados_map[estado_desc]
            
            # Fecha creación: últimos 2 años
            cread = dt_between(365*2, 1) 
            
            # --- LÓGICA V4 (Correcta según tu última instrucción) ---
            
            # 1. Asignar el médico que carga
            dni_carga = random.choice(medicos_dnies)
            
            firm = None
            dni_firma = None
            
            if estado_desc == "Verificado":
                # Si está verificado, la fecha de firma es posterior a la creación
                firm = cread + timedelta(hours=random.randint(1, 72))
                
                # 2. El mismo médico que carga es el que firma.
                dni_firma = dni_carga
            
            # --- FIN DE LA LÓGICA ---

            obs = None if random.random() < 0.7 else fake.sentence(nb_words=8)
            
            analisis_rows.append((id_tipo, id_estado, dni_p, dni_carga, dni_firma, cread, firm, obs))

    # Insertar análisis y recuperar sus IDs y tipos
    inserted_analisis_info = [] 
    
    print(f"  ... Insertando {len(analisis_rows)} análisis...")
    
    for row in analisis_rows:
        cur.execute("""
            INSERT INTO dbo.analisis(id_tipo_analisis, id_estado, dni_paciente, dni_carga, dni_firma, fecha_creacion, fecha_firma, observaciones)
            OUTPUT INSERTED.id_analisis
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """, row)
        new_id = cur.fetchone()[0]
        inserted_analisis_info.append((new_id, row[0])) # (new_id, id_tipo_analisis)

    # Crear 'analisis_metrica' por cada análisis insertado
    rows_am = []
    for id_an, id_tipo in inserted_analisis_info:
        metricas_del_tipo = tipo_metrica_map_ids.get(id_tipo, [])
        if not metricas_del_tipo:
            continue
            
        for mid in metricas_del_tipo:
            lo, hi = rangos_metricas.get(mid, (decimal.Decimal(0), decimal.Decimal(100)))
            
            if random.random() < 0.85:
                val = round(random.uniform(float(lo), float(hi)), 2)
            else:
                val = round(random.uniform(float(lo * decimal.Decimal(0.7)), float(hi * decimal.Decimal(1.3))), 2)

            obs = None if random.random() < 0.9 else fake.sentence(nb_words=5)
            rows_am.append((id_an, mid, val, obs))

    print(f"  ... Insertando {len(rows_am)} resultados (analisis_metrica)...")
    
    cur.executemany("""
        INSERT INTO dbo.analisis_metrica(id_analisis, id_metrica, resultado, observaciones)
        VALUES (?, ?, ?, ?)
    """, rows_am)

# =========================
# ORQUESTADOR (v7)
# =========================
def main():
    conn = None
    try:
        conn = get_connection()
        cur = conn.cursor()

        print("▶ INICIANDO SEED (v7 - Commits Intermedios)...")
        
        # --- PASO 1 ---
        print("▶[1/7] Limpiando base de datos...")
        clear_data(cur)
        conn.commit()
        print("  ... Limpieza GUARDADA (COMMIT).")

        # --- PASO 2 ---
        print("▶[2/7] Insertando Catálogos (Roles, Estados)...")
        roles_map = seed_roles(cur) # Se sigue ejecutando para crear la tabla
        estados_map = seed_estados_analisis(cur)
        conn.commit()
        print("  ... Catálogos GUARDADOS (COMMIT).")

        # --- PASO 3 ---
        print("▶[3/7] Insertando Obras Sociales...")
        obras_ids = seed_obras_sociales(cur)
        conn.commit()
        print("  ... Obras Sociales GUARDADAS (COMMIT).")

        # --- PASO 4 ---
        print("▶[4/7] Insertando Métricas y Tipos de Análisis...")
        metricas_map_nombre = seed_metricas(cur)
        tipos_map_desc = seed_tipos_analisis(cur)
        tipo_metrica_map_ids = seed_tam(cur, tipos_map_desc, metricas_map_nombre)
        conn.commit()
        print("  ... Métricas/Tipos/Relaciones GUARDADOS (COMMIT).")


        # --- PASO 5 (CRÍTICO) ---
        print("▶[5/7] Insertando Usuarios (Admins, Medicos, Asistentes)...")
        usuarios_dnies, medicos_dnies = seed_usuarios_y_extensiones(cur, roles_map)
        conn.commit()
        print("  ... Usuarios GUARDADOS (COMMIT).")

        # --- PASO 6 ---
        print("▶[6/7] Insertando Pacientes...")
        pacientes_dnies = seed_pacientes(cur, obras_ids, usuarios_dnies)
        conn.commit()
        print("  ... Pacientes GUARDADOS (COMMIT).")

        # --- PASO 7 ---
        print("▶[7/7] Insertando Análisis y Resultados...")
        seed_analisis_y_metricas(
            cur, 
            pacientes_dnies, 
            medicos_dnies, 
            tipos_map_desc, 
            estados_map, 
            tipo_metrica_map_ids,
            metricas_map_nombre
        )
        conn.commit()
        print("  ... Análisis y Métricas GUARDADOS (COMMIT).")

        print("\n✅ Seed completado con éxito (TODOS LOS PASOS GUARDADOS).")

    except pyodbc.Error as e:
        print(f"\n❌ ERROR de Base de Datos en algún paso: {e}")
        print("  ... Se hizo ROLLBACK del ÚLTIMO paso. Los pasos anteriores YA ESTÁN GUARDADOS.")
        if conn:
            conn.rollback()
    except Exception as e:
        print(f"\n❌ ERROR Inesperado de Python: {e}")
        print("  ... Se hizo ROLLBACK del ÚLTIMO paso. Los pasos anteriores YA ESTÁN GUARDADOS.")
        if conn:
            conn.rollback()
        raise
    finally:
        if conn:
            conn.close()
            print("  ... Conexión cerrada.")

if __name__ == "__main__":
    main()