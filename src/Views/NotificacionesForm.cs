// Views/NotificacionesForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class NotificacionesForm : Form
    {
        public NotificacionesForm()
        {
            Text = "SALC - Notificaciones";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "Notificaciones",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Listado y envío de notificaciones (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);
        }
    }
}
