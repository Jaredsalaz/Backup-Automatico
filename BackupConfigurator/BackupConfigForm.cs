using BackupCore.Models;
using BackupCore.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupConfigurator
{
    public partial class BackupConfigForm : Form
    {
        private readonly IConfigurationService _configService;
        private BackupConfig? _config;

        // Controls
        private TextBox txtName = null!;
        private ComboBox cmbDatabase = null!;
        private ComboBox cmbClient = null!;
        private ComboBox cmbHour = null!;
        private ComboBox cmbMinute = null!;
        private CheckBox chkEnabled = null!;
        private NumericUpDown nudRetentionDays = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Label lblInfoPath = null!; // Hacer la etiqueta de ruta accesible globalmente

        public BackupConfigForm(IConfigurationService configService, BackupConfig? config = null)
        {
            _configService = configService;
            _config = config;
            InitializeComponent();
            
            // Cargar datos de forma asíncrona después de inicializar la UI
            this.Load += async (s, e) => await LoadDataAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = _config == null ? "Nueva Configuración de Backup" : "Editar Configuración de Backup";
            this.Size = new Size(500, 400); // Aumentar altura
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Name
            var lblName = new Label { Text = "Nombre de la Configuración:", Location = new Point(15, 15), Size = new Size(150, 23) };
            txtName = new TextBox { Location = new Point(170, 12), Size = new Size(300, 23) };

            // Database
            var lblDatabase = new Label { Text = "Base de Datos:", Location = new Point(15, 50), Size = new Size(150, 23) };
            cmbDatabase = new ComboBox 
            { 
                Location = new Point(170, 47), 
                Size = new Size(300, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Client/Branch
            var lblClient = new Label { Text = "Cliente/Sucursal:", Location = new Point(15, 85), Size = new Size(150, 23) };
            cmbClient = new ComboBox 
            { 
                Location = new Point(170, 82), 
                Size = new Size(300, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Schedule
            var lblSchedule = new Label { Text = "Horario de Backup:", Location = new Point(15, 120), Size = new Size(150, 23) };
            
            var lblHour = new Label { Text = "Hora:", Location = new Point(170, 120), Size = new Size(40, 23) };
            cmbHour = new ComboBox 
            { 
                Location = new Point(215, 117), 
                Size = new Size(60, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            // Llenar horas (0-23)
            for (int i = 0; i < 24; i++)
            {
                cmbHour.Items.Add(i.ToString("00"));
            }
            cmbHour.SelectedIndex = 2; // 02:00 por defecto

            var lblMinute = new Label { Text = "Minutos:", Location = new Point(285, 120), Size = new Size(50, 23) };
            cmbMinute = new ComboBox 
            { 
                Location = new Point(340, 117), 
                Size = new Size(60, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            // Llenar minutos (0, 15, 30, 45)
            cmbMinute.Items.AddRange(new[] { "00", "15", "30", "45" });
            cmbMinute.SelectedIndex = 0; // 00 por defecto

            var lblDaily = new Label 
            { 
                Text = "(Se ejecutará todos los días a esta hora)", 
                Location = new Point(170, 145), 
                Size = new Size(250, 23),
                ForeColor = SystemColors.ControlDarkDark,
                Font = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Italic)
            };

            // Retention
            var lblRetention = new Label { Text = "Retener Backups (días):", Location = new Point(15, 180), Size = new Size(150, 23) };
            nudRetentionDays = new NumericUpDown 
            { 
                Location = new Point(170, 177), 
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 365,
                Value = 30
            };

            var lblRetentionInfo = new Label 
            { 
                Text = "Los backups más antiguos se eliminarán automáticamente", 
                Location = new Point(260, 180), 
                Size = new Size(200, 23),
                ForeColor = SystemColors.ControlDarkDark,
                Font = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Italic)
            };

            // Enabled
            chkEnabled = new CheckBox 
            { 
                Text = "Configuración Activa", 
                Location = new Point(170, 215), 
                Size = new Size(150, 23),
                Checked = true
            };

            // Info Panel
            var pnlInfo = new Panel 
            { 
                Location = new Point(15, 250), 
                Size = new Size(455, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightBlue
            };

            var lblInfoTitle = new Label 
            { 
                Text = "Estructura de Archivos:", 
                Location = new Point(5, 5), 
                Size = new Size(120, 15),
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblInfoPath = new Label 
            { 
                Text = "[BasePath]/[Cliente-Sucursal]/backups/BaseDatos_YYYY-MM-DD_HH-mm-ss.ext", 
                Location = new Point(5, 25), 
                Size = new Size(440, 15),
                Font = new Font("Consolas", 8),
                ForeColor = Color.DarkBlue
            };

            pnlInfo.Controls.AddRange(new Control[] { lblInfoTitle, lblInfoPath });

            // Buttons - Mover más abajo
            btnSave = new Button 
            { 
                Text = "Guardar", 
                Location = new Point(300, 320), // Mover más abajo
                Size = new Size(80, 30),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button 
            { 
                Text = "Cancelar", 
                Location = new Point(390, 320), // Mover más abajo
                Size = new Size(80, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblDatabase, cmbDatabase,
                lblClient, cmbClient,
                lblSchedule, lblHour, cmbHour, lblMinute, cmbMinute, lblDaily,
                lblRetention, nudRetentionDays, lblRetentionInfo,
                chkEnabled,
                pnlInfo,
                btnSave, btnCancel
            });

            this.ResumeLayout(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Cargar bases de datos
                var databases = await _configService.GetDatabaseConfigsAsync();
                foreach (var db in databases)
                {
                    cmbDatabase.Items.Add(new ComboBoxItem { Text = db.Name, Value = db });
                }

                // Cargar clientes/sucursales
                var ftpConfigs = await _configService.GetFtpConfigsAsync();
                foreach (var ftp in ftpConfigs)
                {
                    cmbClient.Items.Add(new ComboBoxItem { Text = ftp.Name, Value = ftp });
                }

                // Cargar datos existentes si es edición
                if (_config != null)
                {
                    txtName.Text = _config.Name;
                    chkEnabled.Checked = _config.IsEnabled;
                    nudRetentionDays.Value = _config.RetentionDays;

                    // Buscar la base de datos
                    var dbItem = cmbDatabase.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => ((DatabaseConfig)item.Value).Name == _config.DatabaseConfigName);
                    if (dbItem != null)
                        cmbDatabase.SelectedItem = dbItem;

                    // Buscar el cliente
                    var ftpItem = cmbClient.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => ((FtpConfig)item.Value).Name == _config.FtpConfigName);
                    if (ftpItem != null)
                        cmbClient.SelectedItem = ftpItem;

                    // Parsear horario (formato: "0 2 * * *" = todos los días a las 2:00)
                    if (!string.IsNullOrEmpty(_config.Schedule))
                    {
                        var parts = _config.Schedule.Split(' ');
                        if (parts.Length >= 2)
                        {
                            if (int.TryParse(parts[0], out int minute))
                            {
                                var minuteIndex = cmbMinute.Items.IndexOf(minute.ToString("00"));
                                if (minuteIndex >= 0) cmbMinute.SelectedIndex = minuteIndex;
                            }
                            
                            if (int.TryParse(parts[1], out int hour))
                            {
                                cmbHour.SelectedIndex = hour;
                            }
                        }
                    }
                }

                // Auto-generar nombre si es nuevo
                if (_config == null && cmbDatabase.Items.Count > 0 && cmbClient.Items.Count > 0)
                {
                    cmbDatabase.SelectedIndex = 0;
                    cmbClient.SelectedIndex = 0;
                    UpdateAutoName();
                }
                
                // Actualizar previsualización si es edición
                if (_config != null)
                {
                    UpdatePathPreview();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando datos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Eventos para auto-generar nombre y actualizar previsualización
            cmbDatabase.SelectedIndexChanged += (s, e) => UpdateAutoName();
            cmbClient.SelectedIndexChanged += (s, e) => UpdateAutoName();
        }

        private void UpdateAutoName()
        {
            if (cmbDatabase.SelectedItem != null && cmbClient.SelectedItem != null && string.IsNullOrEmpty(txtName.Text))
            {
                var dbName = ((DatabaseConfig)((ComboBoxItem)cmbDatabase.SelectedItem).Value).Name;
                var clientName = ((FtpConfig)((ComboBoxItem)cmbClient.SelectedItem).Value).Branch;
                txtName.Text = $"Backup {dbName} - {clientName}";
            }
            
            // Actualizar también la previsualización de la ruta
            UpdatePathPreview();
        }

        private void UpdatePathPreview()
        {
            if (cmbDatabase.SelectedItem != null && cmbClient.SelectedItem != null)
            {
                var dbName = ((DatabaseConfig)((ComboBoxItem)cmbDatabase.SelectedItem).Value).Name;
                var ftpConfig = (FtpConfig)((ComboBoxItem)cmbClient.SelectedItem).Value;
                var basePath = ftpConfig.BasePath;
                var branch = ftpConfig.Branch;
                
                // Determinar extensión según tipo de BD
                var dbConfig = (DatabaseConfig)((ComboBoxItem)cmbDatabase.SelectedItem).Value;
                var extension = dbConfig.Type switch
                {
                    DatabaseType.SqlServer => "bak",
                    DatabaseType.MySQL => "sql",
                    DatabaseType.PostgreSQL => "sql",
                    _ => "bak"
                };
                
                lblInfoPath.Text = $"{basePath}/{branch}/backups/{dbName}_YYYY-MM-DD_HH-mm-ss.{extension}";
            }
            else
            {
                lblInfoPath.Text = "[BasePath]/[Cliente-Sucursal]/backups/BaseDatos_YYYY-MM-DD_HH-mm-ss.ext";
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                var config = CreateConfigFromForm();
                
                if (_config == null)
                {
                    await _configService.SaveBackupConfigAsync(config);
                }
                else
                {
                    config.Id = _config.Id;
                    await _configService.UpdateBackupConfigAsync(config);
                }

                MessageBox.Show("Configuración de backup guardada correctamente!", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando configuración: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("El nombre de la configuración es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (cmbDatabase.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una base de datos.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDatabase.Focus();
                return false;
            }

            if (cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente/sucursal.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbClient.Focus();
                return false;
            }

            if (cmbHour.SelectedItem == null || cmbMinute.SelectedItem == null)
            {
                MessageBox.Show("Debe configurar el horario del backup.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private BackupConfig CreateConfigFromForm()
        {
            var selectedDb = (DatabaseConfig)((ComboBoxItem)cmbDatabase.SelectedItem!).Value;
            var selectedFtp = (FtpConfig)((ComboBoxItem)cmbClient.SelectedItem!).Value;
            
            // Crear expresión Cron para ejecución diaria
            var minute = cmbMinute.SelectedItem!.ToString();
            var hour = cmbHour.SelectedItem!.ToString();
            var cronExpression = $"{minute} {hour} * * *"; // Todos los días

            return new BackupConfig
            {
                Id = _config?.Id ?? Guid.NewGuid(),
                Name = txtName.Text.Trim(),
                DatabaseConfigName = selectedDb.Name,
                FtpConfigName = selectedFtp.Name,
                Schedule = cronExpression,
                IsEnabled = chkEnabled.Checked,
                RetentionDays = (int)nudRetentionDays.Value,
                CreatedAt = _config?.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private class ComboBoxItem
        {
            public string Text { get; set; } = "";
            public object Value { get; set; } = null!;
            public override string ToString() => Text;
        }
    }
}
