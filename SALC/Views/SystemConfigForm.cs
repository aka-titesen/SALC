// Views/SystemConfigForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class SystemConfigForm : Form
    {
        public SystemConfigForm()
        {
            Text = "SALC - Configuración del Sistema";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "?? Configuración del Sistema (UI placeholder)",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Parámetros, tipos de análisis, rangos de referencia (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);
        }
    }
}