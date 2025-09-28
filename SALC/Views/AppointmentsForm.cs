// Views/AppointmentsForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    public class AppointmentsForm : Form
    {
        public AppointmentsForm()
        {
            Text = "SALC - Turnos";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            BackColor = Color.White;

            var title = new Label
            {
                Text = "?? Gestión de Turnos",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            Controls.Add(title);

            var info = new Label
            {
                Text = "Agenda, filtros por fecha/paciente/doctor (solo UI)",
                AutoSize = true,
                Location = new Point(20, 60)
            };
            Controls.Add(info);

            // Contenedor de lista (placeholder)
            var listBox = new ListView
            {
                View = View.Details,
                Location = new Point(20, 100),
                Size = new Size(840, 380),
                FullRowSelect = true
            };
            listBox.Columns.Add("ID", 60);
            listBox.Columns.Add("Fecha", 120);
            listBox.Columns.Add("Hora", 100);
            listBox.Columns.Add("Paciente", 220);
            listBox.Columns.Add("Doctor", 220);
            listBox.Columns.Add("Estado", 120);
            Controls.Add(listBox);

            // Botonera (placeholder)
            var pnlButtons = new Panel { Location = new Point(20, 500), Size = new Size(840, 40) };
            var btnNuevo = new Button { Text = "Nuevo", Location = new Point(0, 5), Size = new Size(100, 30) };
            var btnEditar = new Button { Text = "Editar", Location = new Point(110, 5), Size = new Size(100, 30) };
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(220, 5), Size = new Size(100, 30) };
            pnlButtons.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnCancelar });
            Controls.Add(pnlButtons);
        }
    }
}