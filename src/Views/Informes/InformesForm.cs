// Views/Informes/InformesForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class InformesForm : Form
    {
        public InformesForm()
        {
            Text = "SALC - Informes";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "Generación de Informes",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Listado y previsualización de informes PDF (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);
        }
    }
}
