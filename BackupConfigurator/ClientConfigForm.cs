using BackupCore.Models;
using BackupCore.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using FluentFTP;
using FtpConfig = BackupCore.Models.FtpConfig;

namespace BackupConfigurator
{
    public partial class ClientConfigForm : Form
    {
        private readonly IConfigurationService _configService;
        private FtpConfig? _config;

        // Controls
        private TextBox txtClientName = null!;
        private TextBox txtBranchName = null!;
        private TextBox txtHost = null!;
        private NumericUpDown nudPort = null!;
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private CheckBox chkUseSsl = null!;
        private TextBox txtBasePath = null!;
        private Button btnTest = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public ClientConfigForm(IConfigurationService configService, FtpConfig? config = null)
        {
            _configService = configService;
            _config = config;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = _config == null ? "Nueva Configuración de Cliente" : "Editar Configuración de Cliente";
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Client Name
            var lblClient = new Label { Text = "Nombre del Cliente/Empresa:", Location = new Point(15, 15), Size = new Size(150, 23) };
            txtClientName = new TextBox { Location = new Point(170, 12), Size = new Size(300, 23) };

            // Branch Name
            var lblBranch = new Label { Text = "Nombre de la Sucursal:", Location = new Point(15, 50), Size = new Size(150, 23) };
            txtBranchName = new TextBox { Location = new Point(170, 47), Size = new Size(300, 23) };

            // Separator
            var lblSeparator = new Label 
            { 
                Text = "Configuración del Servidor FTP:", 
                Location = new Point(15, 85), 
                Size = new Size(200, 23),
                Font = new Font(this.Font, FontStyle.Bold)
            };

            // Host (pre-configurado)
            var lblHost = new Label { Text = "Servidor FTP:", Location = new Point(15, 115), Size = new Size(100, 23) };
            txtHost = new TextBox 
            { 
                Location = new Point(120, 112), 
                Size = new Size(200, 23),
                Text = "ftp.ejemplo.com",
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            // Port (pre-configurado)
            var lblPort = new Label { Text = "Puerto:", Location = new Point(330, 115), Size = new Size(50, 23) };
            nudPort = new NumericUpDown 
            { 
                Location = new Point(385, 112), 
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 65535,
                Value = 21,
                ReadOnly = true
            };

            // Username (pre-configurado)
            var lblUsername = new Label { Text = "Usuario FTP:", Location = new Point(15, 150), Size = new Size(100, 23) };
            txtUsername = new TextBox 
            { 
                Location = new Point(120, 147), 
                Size = new Size(350, 23),
                Text = "usuario@ejemplo.com",
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            // Password (pre-configurado)
            var lblPassword = new Label { Text = "Contraseña:", Location = new Point(15, 185), Size = new Size(100, 23) };
            txtPassword = new TextBox 
            { 
                Location = new Point(120, 182), 
                Size = new Size(350, 23),
                UseSystemPasswordChar = true,
                Text = "Sistemas2025-A",
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            // SSL (pre-configurado)
            chkUseSsl = new CheckBox 
            { 
                Text = "Usar SSL/TLS (FTPS)", 
                Location = new Point(120, 220), 
                Size = new Size(150, 23),
                Checked = true,
                Enabled = false
            };

            // Base Path
            var lblBasePath = new Label { Text = "Carpeta Base:", Location = new Point(15, 255), Size = new Size(100, 23) };
            txtBasePath = new TextBox 
            { 
                Location = new Point(120, 252), 
                Size = new Size(350, 23),
                Text = "/SantaCruz"
            };

            // Info Label
            var lblInfo = new Label 
            { 
                Name = "lblInfo",
                Text = "Los archivos se guardarán en: /SantaCruz/[Cliente]-[Sucursal]/backups/",
                Location = new Point(15, 285), 
                Size = new Size(450, 23),
                ForeColor = SystemColors.ControlDarkDark,
                Font = new Font(this.Font.FontFamily, this.Font.Size - 1, FontStyle.Italic)
            };

            // Event handlers para actualizar el preview
            txtClientName.TextChanged += UpdatePreviewPath;
            txtBranchName.TextChanged += UpdatePreviewPath;
            txtBasePath.TextChanged += UpdatePreviewPath;

            // Buttons
            btnTest = new Button 
            { 
                Text = "Probar Conexión FTP", 
                Location = new Point(15, 320), 
                Size = new Size(130, 30)
            };
            btnTest.Click += BtnTest_Click;

            btnSave = new Button 
            { 
                Text = "Guardar", 
                Location = new Point(300, 320), 
                Size = new Size(80, 30)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button 
            { 
                Text = "Cancelar", 
                Location = new Point(390, 320), 
                Size = new Size(80, 30)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                lblClient, txtClientName,
                lblBranch, txtBranchName,
                lblSeparator,
                lblHost, txtHost,
                lblPort, nudPort,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                chkUseSsl,
                lblBasePath, txtBasePath,
                lblInfo,
                btnTest, btnSave, btnCancel
            });

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            if (_config != null)
            {
                // Extraer el nombre del cliente del Branch si está configurado
                var parts = _config.Branch.Split('-');
                if (parts.Length >= 2)
                {
                    txtClientName.Text = parts[0];
                    txtBranchName.Text = string.Join("-", parts.Skip(1));
                }
                else
                {
                    txtBranchName.Text = _config.Branch;
                }
                
                // Cargar la carpeta base
                txtBasePath.Text = _config.BasePath;
            }
            
            // Actualizar el preview inicial
            UpdatePreviewPath(null, EventArgs.Empty);
        }

        private void UpdatePreviewPath(object? sender, EventArgs e)
        {
            var lblInfo = this.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblInfo");
            if (lblInfo != null)
            {
                var basePath = string.IsNullOrWhiteSpace(txtBasePath.Text) ? "[Carpeta Base]" : txtBasePath.Text;
                var cliente = string.IsNullOrWhiteSpace(txtClientName.Text) ? "[Cliente]" : txtClientName.Text;
                var sucursal = string.IsNullOrWhiteSpace(txtBranchName.Text) ? "[Sucursal]" : txtBranchName.Text;
                
                lblInfo.Text = $"Los archivos se guardarán en: {basePath}/{cliente}-{sucursal}/backups/";
            }
        }

        private async void BtnTest_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                btnTest.Enabled = false;
                btnTest.Text = "Probando...";

                var testConfig = CreateConfigFromForm();
                
                // Probar conexión FTP real
                await TestFtpConnectionAsync(testConfig);
                
                MessageBox.Show("✅ Conexión FTP exitosa!\n\nSe verificó la conectividad al servidor y se creó la estructura de carpetas correctamente.", "Prueba de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error en la conexión FTP:\n{ex.Message}", "Error de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "Probar Conexión FTP";
            }
        }

        private async Task TestFtpConnectionAsync(FtpConfig config)
        {
            await Task.Run(() =>
            {
                using var client = new FtpClient(config.Host, config.Username, config.Password, config.Port);
                
                // Configurar SSL si está habilitado
                if (config.UseSsl)
                {
                    client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                    client.Config.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    client.Config.ValidateAnyCertificate = true;
                }

                // Conectar al servidor
                client.Connect();

                // Verificar que podemos navegar al directorio base
                var fullPath = $"{config.BasePath}/{config.Branch}/backups";
                
                // Crear la estructura de directorios si no existe
                client.CreateDirectory(fullPath, true);
                
                // Verificar que el directorio existe
                var exists = client.DirectoryExists(fullPath);
                if (!exists)
                {
                    throw new Exception($"No se pudo crear o acceder al directorio: {fullPath}");
                }
                
                client.Disconnect();
            });
        }

        // Método público para pruebas desde otros formularios
        public async Task TestConnectionDirectly(FtpConfig config)
        {
            await TestFtpConnectionAsync(config);
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
                    await _configService.SaveFtpConfigAsync(config);
                }
                else
                {
                    config.Id = _config.Id;
                    await _configService.UpdateFtpConfigAsync(config);
                }

                MessageBox.Show("Configuración de cliente guardada correctamente!", "Éxito", 
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
            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                MessageBox.Show("El nombre del cliente/empresa es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClientName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBranchName.Text))
            {
                MessageBox.Show("El nombre de la sucursal es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBranchName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBasePath.Text))
            {
                MessageBox.Show("La carpeta base es obligatoria.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBasePath.Focus();
                return false;
            }

            // Validar que la carpeta base comience con /
            if (!txtBasePath.Text.StartsWith("/"))
            {
                MessageBox.Show("La carpeta base debe comenzar con '/' (ejemplo: /SantaCruz)", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBasePath.Focus();
                return false;
            }

            return true;
        }

        private FtpConfig CreateConfigFromForm()
        {
            var branch = $"{txtClientName.Text.Trim()}-{txtBranchName.Text.Trim()}";
            
            return new FtpConfig
            {
                Id = _config?.Id ?? Guid.NewGuid(),
                Name = $"FTP {txtClientName.Text.Trim()} - {txtBranchName.Text.Trim()}",
                Host = txtHost.Text.Trim(),
                Port = (int)nudPort.Value,
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text, // Se encriptará en el servicio
                Branch = branch,
                BasePath = txtBasePath.Text.Trim(),
                UseSsl = chkUseSsl.Checked,
                CreatedAt = _config?.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
