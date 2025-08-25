using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using BackupCore.Interfaces;

namespace BackupConfigurator
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MainForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Configurador de Backup Automático";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            
            var label = new Label
            {
                Text = "Sistema de Backup Automático\n\nTodo funcionando correctamente.\nInterfaz gráfica en desarrollo.",
                Location = new Point(50, 100),
                Size = new Size(900, 200),
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            this.Controls.Add(label);
        }
    }
}
