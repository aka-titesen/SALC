// Views/BackupsForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class BackupsForm : Form
    {
        public BackupsForm()
        {
            Text = "SALC - Copias de Seguridad";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "?? Copias de Seguridad (UI placeholder)",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Programación y restauración de backups (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);
        }
    }
}