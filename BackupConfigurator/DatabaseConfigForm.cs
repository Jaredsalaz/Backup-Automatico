using BackupCore.Models;
using BackupCore.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MySql.Data.MySqlClient;
using Npgsql;

namespace BackupConfigurator
{
    public partial class DatabaseConfigForm : Form
    {
        private readonly IConfigurationService _configService;
        private DatabaseConfig? _config;

        // Controls
        private TextBox txtName = null!;
        private ComboBox cmbType = null!;
        private TextBox txtServer = null!;
        private TextBox txtDatabase = null!;
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private NumericUpDown nudPort = null!;
        private Button btnTest = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public DatabaseConfigForm(IConfigurationService configService, DatabaseConfig? config = null)
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
            this.Text = _config == null ? "Nueva Base de Datos" : "Editar Base de Datos";
            this.Size = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Name
            var lblName = new Label { Text = "Nombre:", Location = new Point(15, 15), Size = new Size(80, 23) };
            txtName = new TextBox { Location = new Point(100, 12), Size = new Size(320, 23) };

            // Type
            var lblType = new Label { Text = "Tipo:", Location = new Point(15, 50), Size = new Size(80, 23) };
            cmbType = new ComboBox 
            { 
                Location = new Point(100, 47), 
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new[] { "SqlServer", "MySQL", "PostgreSQL" });
            cmbType.SelectedIndex = 0;

            // Server
            var lblServer = new Label { Text = "Servidor:", Location = new Point(15, 85), Size = new Size(80, 23) };
            txtServer = new TextBox { Location = new Point(100, 82), Size = new Size(220, 23) };

            // Port
            var lblPort = new Label { Text = "Puerto:", Location = new Point(330, 85), Size = new Size(50, 23) };
            nudPort = new NumericUpDown 
            { 
                Location = new Point(385, 82), 
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 65535,
                Value = 1433
            };

            // Database
            var lblDatabase = new Label { Text = "Base de Datos:", Location = new Point(15, 120), Size = new Size(80, 23) };
            txtDatabase = new TextBox { Location = new Point(100, 117), Size = new Size(320, 23) };

            // Username
            var lblUsername = new Label { Text = "Usuario:", Location = new Point(15, 155), Size = new Size(80, 23) };
            txtUsername = new TextBox { Location = new Point(100, 152), Size = new Size(320, 23) };

            // Password
            var lblPassword = new Label { Text = "Contraseña:", Location = new Point(15, 190), Size = new Size(80, 23) };
            txtPassword = new TextBox 
            { 
                Location = new Point(100, 187), 
                Size = new Size(320, 23),
                UseSystemPasswordChar = true
            };

            // Buttons
            btnTest = new Button 
            { 
                Text = "Probar Conexión", 
                Location = new Point(15, 240), 
                Size = new Size(120, 30)
            };
            btnTest.Click += BtnTest_Click;

            btnSave = new Button 
            { 
                Text = "Guardar", 
                Location = new Point(250, 240), 
                Size = new Size(80, 30)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button 
            { 
                Text = "Cancelar", 
                Location = new Point(340, 240), 
                Size = new Size(80, 30)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblType, cmbType,
                lblServer, txtServer,
                lblPort, nudPort,
                lblDatabase, txtDatabase,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnTest, btnSave, btnCancel
            });

            // Event handlers
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;

            this.ResumeLayout(false);
        }

        private void LoadData()
        {
            if (_config != null)
            {
                txtName.Text = _config.Name;
                cmbType.SelectedItem = _config.Type.ToString();
                txtServer.Text = _config.Server;
                nudPort.Value = _config.Port;
                txtDatabase.Text = _config.Database;
                txtUsername.Text = _config.Username;
                // Password se mantiene encriptada, no se muestra
            }
        }

        private void CmbType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Actualizar puerto por defecto según el tipo
            switch (cmbType.SelectedItem?.ToString())
            {
                case "SqlServer":
                    nudPort.Value = 1433;
                    break;
                case "MySQL":
                    nudPort.Value = 3306;
                    break;
                case "PostgreSQL":
                    nudPort.Value = 5432;
                    break;
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
                
                // Probar conexión real
                await TestDatabaseConnectionAsync(testConfig);
                
                MessageBox.Show("✅ Conexión exitosa!", "Prueba de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error en la conexión:\n{ex.Message}", "Error de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "Probar Conexión";
            }
        }

        private async Task TestDatabaseConnectionAsync(DatabaseConfig config)
        {
            switch (config.Type)
            {
                case DatabaseType.SqlServer:
                    await TestSqlServerConnectionAsync(config);
                    break;
                case DatabaseType.MySQL:
                    await TestMySqlConnectionAsync(config);
                    break;
                case DatabaseType.PostgreSQL:
                    await TestPostgreSqlConnectionAsync(config);
                    break;
                default:
                    throw new NotSupportedException($"Tipo de base de datos no soportado: {config.Type}");
            }
        }

        private async Task TestSqlServerConnectionAsync(DatabaseConfig config)
        {
            using var connection = new System.Data.SqlClient.SqlConnection(config.ConnectionString);
            await connection.OpenAsync();
            
            // Verificar que la base de datos existe
            using var command = new System.Data.SqlClient.SqlCommand(
                "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName", connection);
            command.Parameters.AddWithValue("@dbName", config.Database);
            
            var result = await command.ExecuteScalarAsync();
            var exists = result != null && (int)result > 0;
            if (!exists)
            {
                throw new Exception($"La base de datos '{config.Database}' no existe en el servidor.");
            }
        }

        private async Task TestMySqlConnectionAsync(DatabaseConfig config)
        {
            using var connection = new MySqlConnection(config.ConnectionString);
            await connection.OpenAsync();
            
            // Verificar que la base de datos existe
            using var command = new MySqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @dbName", connection);
            command.Parameters.AddWithValue("@dbName", config.Database);
            
            var result = await command.ExecuteScalarAsync();
            var exists = result != null && Convert.ToInt32(result) > 0;
            if (!exists)
            {
                throw new Exception($"La base de datos '{config.Database}' no existe en el servidor MySQL.");
            }
        }

        private async Task TestPostgreSqlConnectionAsync(DatabaseConfig config)
        {
            using var connection = new NpgsqlConnection(config.ConnectionString);
            await connection.OpenAsync();
            
            // Verificar que la base de datos existe
            using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM pg_database WHERE datname = @dbName", connection);
            command.Parameters.AddWithValue("@dbName", config.Database);
            
            var result = await command.ExecuteScalarAsync();
            var exists = result != null && Convert.ToInt32(result) > 0;
            if (!exists)
            {
                throw new Exception($"La base de datos '{config.Database}' no existe en el servidor PostgreSQL.");
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
                    await _configService.SaveDatabaseConfigAsync(config);
                }
                else
                {
                    config.Id = _config.Id;
                    await _configService.UpdateDatabaseConfigAsync(config);
                }

                MessageBox.Show("Configuración guardada correctamente!", "Éxito", 
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
                MessageBox.Show("El nombre es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MessageBox.Show("El servidor es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServer.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                MessageBox.Show("El nombre de la base de datos es obligatorio.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDatabase.Focus();
                return false;
            }

            return true;
        }

        private DatabaseConfig CreateConfigFromForm()
        {
            return new DatabaseConfig
            {
                Id = _config?.Id ?? Guid.NewGuid(),
                Name = txtName.Text.Trim(),
                Type = Enum.Parse<DatabaseType>(cmbType.SelectedItem!.ToString()!),
                Server = txtServer.Text.Trim(),
                Port = (int)nudPort.Value,
                Database = txtDatabase.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text, // Se encriptará en el servicio
                CreatedAt = _config?.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        // Método público para probar conexión desde otros formularios
        public async Task TestConnectionDirectly(DatabaseConfig config)
        {
            await TestDatabaseConnectionAsync(config);
        }
    }
}
