// Views/HistoryForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class HistoryForm : Form
    {
        public HistoryForm()
        {
            Text = "SALC - Historial de Órdenes";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "?? Historial de Órdenes (UI placeholder)",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Listado de órdenes con filtros y estados (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);
        }
    }
}