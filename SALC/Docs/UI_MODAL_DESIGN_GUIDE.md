# Guía de Diseño Estandarizada para Modales (Formularios de Diálogo)
## Sistema SALC - Versión 1.0

---

## ?? Resumen Ejecutivo

Todos los formularios modales (alta, modificación y visualización de entidades) deben seguir esta guía de diseño unificada para garantizar consistencia visual y mejor experiencia de usuario en todo el sistema SALC.

---

## ?? Especificaciones de Diseño

### 1. **Propiedades del Formulario**

```csharp
// Configuración base para TODOS los modales
FormBorderStyle = FormBorderStyle.FixedDialog;
MaximizeBox = false;
MinimizeBox = false;
StartPosition = FormStartPosition.CenterParent;
BackColor = Color.White;
```

#### Tamaños Recomendados
- **Formularios Simples** (5-8 campos): `Width = 450`, `Height = 500-550`
- **Formularios Medianos** (9-12 campos): `Width = 500`, `Height = 600-700`
- **Formularios Complejos** (+12 campos): `Width = 600`, `Height = 700-800`

---

### 2. **Encabezado del Modal**

#### **Título Principal**
```csharp
var lblTitulo = new Label
{
    Text = "Título Descriptivo de la Acción", // Ej: "Crear Nuevo Usuario", "Editar Paciente"
    Font = new Font("Segoe UI", 14, FontStyle.Bold),
    ForeColor = Color.FromArgb(70, 130, 180), // Azul corporativo
    Location = new Point(20, 15),
    Size = new Size(400, 30),
    BackColor = Color.Transparent
};
```

#### **Subtítulo Descriptivo**
```csharp
var lblSubtitulo = new Label
{
    Text = "Descripción breve de la operación",
    Font = new Font("Segoe UI", 9, FontStyle.Regular),
    ForeColor = Color.FromArgb(127, 140, 141), // Gris descriptivo
    Location = new Point(20, 45),
    Size = new Size(400, 20),
    BackColor = Color.Transparent
};
```

**Posición inicial del primer campo**: `Top = 80`

---

### 3. **Campos de Entrada**

#### **Labels (Etiquetas)**
```csharp
var lblCampo = new Label
{
    Text = "Nombre del Campo:",
    Left = 20,
    Top = Y_POSITION,
    Width = 120,
    Font = new Font("Segoe UI", 10, FontStyle.Bold),
    ForeColor = Color.FromArgb(44, 62, 80)
};
```

#### **TextBox (Entrada de Texto)**
```csharp
var txtCampo = new TextBox
{
    Left = 150,
    Top = Y_POSITION - 2, // Alineado con el label
    Width = 250,
    Font = new Font("Segoe UI", 10),
    BorderStyle = BorderStyle.FixedSingle
};
```

#### **ComboBox (Selector)**
```csharp
var cboCampo = new ComboBox
{
    Left = 150,
    Top = Y_POSITION - 2,
    Width = 250,
    DropDownStyle = ComboBoxStyle.DropDownList,
    Font = new Font("Segoe UI", 10)
};
```

#### **DateTimePicker (Selector de Fecha)**
```csharp
var dtpCampo = new DateTimePicker
{
    Left = 150,
    Top = Y_POSITION - 2,
    Width = 250,
    Format = DateTimePickerFormat.Short,
    Font = new Font("Segoe UI", 10)
};
```

#### **Espaciado Entre Campos**
- **Espaciado estándar**: `35px` entre campos
- **Ejemplo**:
  - Campo 1: Top = 80
  - Campo 2: Top = 115
  - Campo 3: Top = 150
  - etc.

---

### 4. **Campos de Solo Lectura**

#### **Para Campos Bloqueados (ReadOnly)**
```csharp
var lblCampoLectura = new Label
{
    Text = "Valor del campo (solo lectura)",
    Left = 150,
    Top = Y_POSITION,
    Width = 250,
    Height = 30,
    BorderStyle = BorderStyle.FixedSingle,
    BackColor = Color.FromArgb(240, 240, 240), // Fondo gris claro
    ForeColor = Color.FromArgb(44, 62, 80),
    Font = new Font("Segoe UI", 10, FontStyle.Regular),
    TextAlign = ContentAlignment.MiddleLeft,
    Padding = new Padding(10, 0, 0, 0)
};
```

#### **Nota Explicativa (Opcional)**
```csharp
var lblNota = new Label
{
    Text = "Explicación de por qué el campo está bloqueado",
    Left = 20,
    Top = Y_POSITION + 35,
    Width = 400,
    Height = 20,
    Font = new Font("Segoe UI", 8, FontStyle.Italic),
    ForeColor = Color.FromArgb(192, 57, 43), // Rojo suave
    BackColor = Color.Transparent
};
```

---

### 5. **Botones de Acción**

#### **Botón Primario (Aceptar/Guardar/Crear)**
```csharp
btnOk = new Button
{
    Text = "Texto de Acción", // "Guardar Cambios", "Crear Usuario", etc.
    Left = ANCHO_MODAL - 210,  // Posicionado a la derecha
    Top = ALTO_MODAL - 70,     // 70px desde el fondo
    Width = 120,
    Height = 35,
    DialogResult = DialogResult.OK,
    Font = new Font("Segoe UI", 10, FontStyle.Bold),
    BackColor = Color.FromArgb(39, 174, 96), // Verde éxito
    ForeColor = Color.White,
    FlatStyle = FlatStyle.Flat,
    Cursor = Cursors.Hand
};
btnOk.FlatAppearance.BorderSize = 0;
```

#### **Botón Secundario (Cancelar)**
```csharp
btnCancel = new Button
{
    Text = "Cancelar",
    Left = ANCHO_MODAL - 90,   // Al lado del botón primario
    Top = ALTO_MODAL - 70,
    Width = 90,
    Height = 35,
    DialogResult = DialogResult.Cancel,
    Font = new Font("Segoe UI", 10, FontStyle.Regular),
    BackColor = Color.FromArgb(149, 165, 166), // Gris neutro
    ForeColor = Color.White,
    FlatStyle = FlatStyle.Flat,
    Cursor = Cursors.Hand
};
btnCancel.FlatAppearance.BorderSize = 0;
```

#### **Ejemplo de Posicionamiento (Modal de 450x500)**
```csharp
btnOk.Location = new Point(180, 430);     // 450 - 210 - 60 margen
btnCancel.Location = new Point(310, 430); // 450 - 90 - 50 margen
```

---

### 6. **Paleta de Colores Estandarizada**

| Uso | Color RGB | Código | Descripción |
|-----|-----------|--------|-------------|
| **Título Principal** | `(70, 130, 180)` | Azul corporativo | Encabezados principales |
| **Subtítulo** | `(127, 140, 141)` | Gris descriptivo | Texto secundario |
| **Labels** | `(44, 62, 80)` | Gris oscuro | Etiquetas de campos |
| **Botón Primario** | `(39, 174, 96)` | Verde éxito | Acciones afirmativas |
| **Botón Secundario** | `(149, 165, 166)` | Gris neutro | Cancelar/Cerrar |
| **Campo Bloqueado** | `(240, 240, 240)` | Gris claro | Fondo de solo lectura |
| **Nota/Advertencia** | `(192, 57, 43)` | Rojo suave | Mensajes de advertencia |
| **Fondo Modal** | `(255, 255, 255)` | Blanco | Fondo de formulario |

---

### 7. **Comportamiento Estandarizado**

#### **Validación de Campos**
```csharp
private bool Validar()
{
    // 1. Validar campos obligatorios
    if (string.IsNullOrWhiteSpace(txtCampo.Text))
    {
        MessageBox.Show("El campo X es obligatorio.", "Validación", 
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtCampo.Focus();
        return false;
    }
    
    // 2. Validar formato (DNI, email, etc.)
    // 3. Validar rangos de valores
    // 4. Validar reglas de negocio
    
    return true;
}
```

#### **Evento de Botón OK**
```csharp
btnOk.Click += (s, e) => 
{
    if (!Validar())
        this.DialogResult = DialogResult.None; // Prevenir cierre si falla validación
};
```

#### **Configuración de Teclas**
```csharp
AcceptButton = btnOk;    // Enter ejecuta OK
CancelButton = btnCancel; // Escape ejecuta Cancelar
```

---

### 8. **Diferencias por Rol**

#### **Administrador**
- ? **Todos los campos editables**
- ? **Puede cambiar estado** (Activo/Inactivo)
- ? **Puede hacer baja lógica** (cambiar a Inactivo)

```csharp
// ComboBox de estado - EDITABLE
cboEstado = new ComboBox
{
    // ... configuración estándar
    Enabled = true
};
cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
```

#### **Médico (Solo para Pacientes)**
- ? **Puede modificar datos** del paciente
- ? **Puede dar de baja** (cambiar estado a Inactivo)
- ? **NO puede crear** nuevos pacientes

```csharp
// Usar mismo formulario del Administrador
// Validar en el Presenter que sea operación de edición
if (existente == null)
    throw new InvalidOperationException("El médico no puede crear pacientes.");
```

#### **Asistente**
- ? **Puede crear** nuevos pacientes (siempre Activo)
- ? **Puede modificar datos** de pacientes existentes
- ? **NO puede modificar estado** (campo de solo lectura)
- ? **NO puede dar de baja**

```csharp
// Label de solo lectura para el estado
lblEstadoActual = new Label
{
    Text = existente?.Estado ?? "Activo",
    // ... estilo de campo bloqueado (ver sección 4)
};

// Nota explicativa
var lblNotaEstado = new Label
{
    Text = "El estado del paciente no puede modificarse desde el rol Asistente",
    // ... estilo de nota (ver sección 4)
};
```

---

### 9. **Formularios de Solo Lectura (Detalle)**

Para formularios de visualización (sin edición):

```csharp
// Deshabilitar TODOS los campos
txtCampo.ReadOnly = true;
txtCampo.BackColor = Color.FromArgb(240, 240, 240);

// Ocultar botón Guardar/Aceptar
btnOk.Visible = false;

// Cambiar texto del botón Cancelar
btnCancel.Text = "Cerrar";
btnCancel.BackColor = Color.FromArgb(70, 130, 180); // Azul corporativo
```

---

### 10. **Mensajes de Validación**

#### **Formato Estandarizado**
```csharp
MessageBox.Show(
    "Mensaje descriptivo del error o validación.",
    "Título del Diálogo", // "Validación", "Error", "Advertencia"
    MessageBoxButtons.OK,
    MessageBoxIcon.Warning // o Error, Information
);
```

#### **Ejemplos**
```csharp
// Campo obligatorio
MessageBox.Show("El nombre es obligatorio.", "Validación", 
    MessageBoxButtons.OK, MessageBoxIcon.Warning);

// Formato inválido
MessageBox.Show("El DNI debe ser un número válido.", "Validación", 
    MessageBoxButtons.OK, MessageBoxIcon.Warning);

// Rango inválido
MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Validación", 
    MessageBoxButtons.OK, MessageBoxIcon.Warning);
```

---

## ? Checklist de Implementación

Al crear o actualizar un modal, verificar:

- [ ] Título y subtítulo descriptivos
- [ ] Campos alineados correctamente (Left = 20 para labels, Left = 150 para inputs)
- [ ] Espaciado de 35px entre campos
- [ ] Fuente Segoe UI, tamaño 10 para campos
- [ ] Botones con colores estandarizados
- [ ] Botones posicionados en esquina inferior derecha
- [ ] Validación completa implementada
- [ ] AcceptButton y CancelButton configurados
- [ ] Campos de solo lectura con fondo gris
- [ ] Restricciones por rol aplicadas correctamente
- [ ] Mensajes de error descriptivos y claros

---

## ?? Plantilla de Código Completa

```csharp
using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;

namespace SALC.Views
{
    public class FrmEntidadEdit : Form
    {
        // Controles privados
        private TextBox txtCampo1, txtCampo2;
        private ComboBox cboCampo3;
        private DateTimePicker dtpCampo4;
        private Button btnOk, btnCancel;

        public FrmEntidadEdit(Entidad existente = null)
        {
            // 1. Configuración del formulario
            Text = existente == null ? "Nueva Entidad" : "Editar Entidad";
            Width = 450;
            Height = 500;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // 2. Encabezado
            var lblTitulo = new Label
            {
                Text = existente == null ? "Crear Nueva Entidad" : "Modificar Entidad",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };

            var lblSubtitulo = new Label
            {
                Text = "Complete los datos de la entidad",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(400, 20)
            };

            // 3. Campos (ejemplo)
            var lblCampo1 = new Label 
            { 
                Text = "Campo 1:", 
                Left = 20, 
                Top = 80, 
                Width = 120, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            txtCampo1 = new TextBox 
            { 
                Left = 150, 
                Top = 78, 
                Width = 250, 
                Font = new Font("Segoe UI", 10) 
            };

            // ... más campos con espaciado de 35px

            // 4. Botones
            btnOk = new Button
            {
                Text = existente == null ? "Crear" : "Guardar Cambios",
                Left = 180,
                Top = 430,
                Width = 120,
                Height = 35,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Left = 310,
                Top = 430,
                Width = 90,
                Height = 35,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            // 5. Configuración de teclas
            AcceptButton = btnOk;
            CancelButton = btnCancel;
            btnOk.Click += (s, e) => { if (!Validar()) this.DialogResult = DialogResult.None; };

            // 6. Agregar controles
            Controls.AddRange(new Control[] {
                lblTitulo, lblSubtitulo,
                lblCampo1, txtCampo1,
                // ... más campos
                btnOk, btnCancel
            });

            // 7. Cargar datos si es edición
            if (existente != null)
            {
                txtCampo1.Text = existente.Campo1;
                // ... cargar más campos
            }
        }

        private bool Validar()
        {
            // Implementar validaciones
            return true;
        }

        public Entidad ObtenerEntidad()
        {
            return new Entidad
            {
                Campo1 = txtCampo1.Text.Trim(),
                // ... más campos
            };
        }
    }
}
```

---

## ?? Objetivo

**Todos los modales del sistema SALC deben seguir esta guía para garantizar**:
- ? Consistencia visual en toda la aplicación
- ? Mejor experiencia de usuario
- ? Mantenibilidad del código
- ? Cumplimiento de restricciones por rol

---

**Versión**: 1.0  
**Fecha**: 2025  
**Autor**: Sistema SALC  
