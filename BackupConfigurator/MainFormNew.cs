using BackupCore.Interfaces;
using BackupCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupConfigurator
{
    public partial class MainFormNew : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigurationService _configService;
        private readonly IBackupLogService _logService;

        private TabControl tabControl = null!;
        private TabPage tabDatabases = null!;
        private TabPage tabFtp = null!;
        private TabPage tabBackups = null!;
        private TabPage tabLogs = null!;

        public MainFormNew(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configService = serviceProvider.GetRequiredService<IConfigurationService>();
            _logService = serviceProvider.GetRequiredService<IBackupLogService>();
            
            InitializeComponent();
            SetupTabsAndContent();
            
            this.Load += async (s, e) => await LoadAllDataAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // MainForm
            this.Text = "🚀 Configurador de Backup Automático";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.Icon = SystemIcons.Application;
            
            this.ResumeLayout(false);
        }

        private void SetupTabsAndContent()
        {
            // Crear TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ItemSize = new Size(200, 40),
                SizeMode = TabSizeMode.Fixed
            };

            // Crear las pestañas
            CreateDatabaseTab();
            CreateFtpTab();
            CreateBackupTab();
            CreateLogsTab();

            // Agregar pestañas al control
            tabControl.TabPages.Add(tabDatabases);
            tabControl.TabPages.Add(tabFtp);
            tabControl.TabPages.Add(tabBackups);
            tabControl.TabPages.Add(tabLogs);

            // Agregar TabControl al formulario
            this.Controls.Add(tabControl);
        }

        private void CreateDatabaseTab()
        {
            tabDatabases = new TabPage("📊 Bases de Datos");
            tabDatabases.BackColor = Color.White;
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            
            // Título
            var lblTitle = new Label
            {
                Text = "Configuración de Bases de Datos",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Size = new Size(400, 30),
                Location = new Point(15, 15)
            };
            
            // Botón agregar
            var btnAdd = new Button
            {
                Text = "➕ Agregar Nueva Base de Datos",
                Size = new Size(250, 40),
                Location = new Point(15, 55),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAddDatabase_Click;

            // ListView
            var listView = new ListView
            {
                Name = "lvDatabases",
                Location = new Point(15, 105),
                Size = new Size(panel.Width - 30, panel.Height - 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F)
            };

            listView.Columns.Add("Nombre", 200);
            listView.Columns.Add("Tipo", 120);
            listView.Columns.Add("Servidor", 200);
            listView.Columns.Add("Base de Datos", 180);
            listView.Columns.Add("Estado", 100);

            // Agregar menú contextual
            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("✏️ Editar");
            var deleteItem = new ToolStripMenuItem("🗑️ Eliminar");
            var testItem = new ToolStripMenuItem("🔌 Probar Conexión");
            
            editItem.Click += async (s, e) => await EditDatabase_Click(listView);
            deleteItem.Click += async (s, e) => await DeleteDatabase_Click(listView);
            testItem.Click += async (s, e) => await TestDatabase_Click(listView);
            
            contextMenu.Items.AddRange(new ToolStripItem[] { editItem, testItem, deleteItem });
            listView.ContextMenuStrip = contextMenu;
            
            // Doble clic para editar
            listView.DoubleClick += async (s, e) => await EditDatabase_Click(listView);

            panel.Controls.AddRange(new Control[] { lblTitle, btnAdd, listView });
            tabDatabases.Controls.Add(panel);
        }

        private void CreateFtpTab()
        {
            tabFtp = new TabPage("🏢 Clientes/Empresas");
            tabFtp.BackColor = Color.White;
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            
            // Título
            var lblTitle = new Label
            {
                Text = "Configuración de Clientes y Sucursales",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Size = new Size(450, 30),
                Location = new Point(15, 15)
            };
            
            // Botón agregar
            var btnAdd = new Button
            {
                Text = "🏢 Agregar Cliente/Empresa",
                Size = new Size(220, 40),
                Location = new Point(15, 55),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAddFtp_Click;

            // ListView
            var listView = new ListView
            {
                Name = "lvFtp",
                Location = new Point(15, 105),
                Size = new Size(panel.Width - 30, panel.Height - 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F)
            };

            listView.Columns.Add("Cliente/Empresa", 200);
            listView.Columns.Add("Sucursal", 150);
            listView.Columns.Add("Servidor FTP", 150);
            listView.Columns.Add("Puerto", 80);
            listView.Columns.Add("SSL", 60);
            listView.Columns.Add("Estado", 100);

            // Agregar menú contextual
            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("✏️ Editar");
            var deleteItem = new ToolStripMenuItem("🗑️ Eliminar");
            var testItem = new ToolStripMenuItem("🔌 Probar Conexión FTP");
            
            editItem.Click += async (s, e) => await EditFtp_Click(listView);
            deleteItem.Click += async (s, e) => await DeleteFtp_Click(listView);
            testItem.Click += async (s, e) => await TestFtp_Click(listView);
            
            contextMenu.Items.AddRange(new ToolStripItem[] { editItem, testItem, deleteItem });
            listView.ContextMenuStrip = contextMenu;
            
            // Doble clic para editar
            listView.DoubleClick += async (s, e) => await EditFtp_Click(listView);

            panel.Controls.AddRange(new Control[] { lblTitle, btnAdd, listView });
            tabFtp.Controls.Add(panel);
        }

        private void CreateBackupTab()
        {
            tabBackups = new TabPage("⚙️ Configuraciones");
            tabBackups.BackColor = Color.White;
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            
            // Título
            var lblTitle = new Label
            {
                Text = "Configuraciones de Backup Automático",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Size = new Size(500, 30),
                Location = new Point(15, 15)
            };
            
            // Botón agregar
            var btnAdd = new Button
            {
                Text = "⚙️ Crear Nueva Configuración",
                Size = new Size(230, 40),
                Location = new Point(15, 55),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAddBackup_Click;

            // ListView
            var listView = new ListView
            {
                Name = "lvBackups",
                Location = new Point(15, 105),
                Size = new Size(panel.Width - 30, panel.Height - 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F)
            };

            listView.Columns.Add("Nombre", 200);
            listView.Columns.Add("Base de Datos", 150);
            listView.Columns.Add("Cliente", 150);
            listView.Columns.Add("Horario", 100);
            listView.Columns.Add("Próximo Backup", 130);
            listView.Columns.Add("Estado", 100);

            // Agregar menú contextual
            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("✏️ Editar");
            var deleteItem = new ToolStripMenuItem("🗑️ Eliminar");
            var enableItem = new ToolStripMenuItem("🔄 Activar/Desactivar");
            
            editItem.Click += async (s, e) => await EditBackup_Click(listView);
            deleteItem.Click += async (s, e) => await DeleteBackup_Click(listView);
            enableItem.Click += async (s, e) => await ToggleBackup_Click(listView);
            
            contextMenu.Items.AddRange(new ToolStripItem[] { editItem, enableItem, deleteItem });
            listView.ContextMenuStrip = contextMenu;
            
            // Doble clic para editar
            listView.DoubleClick += async (s, e) => await EditBackup_Click(listView);

            panel.Controls.AddRange(new Control[] { lblTitle, btnAdd, listView });
            tabBackups.Controls.Add(panel);
        }

        private void CreateLogsTab()
        {
            tabLogs = new TabPage("📋 Logs del Sistema");
            tabLogs.BackColor = Color.White;
            
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
            
            // Título
            var lblTitle = new Label
            {
                Text = "Logs y Historial del Sistema",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Size = new Size(400, 30),
                Location = new Point(15, 15)
            };
            
            // Botón refrescar
            var btnRefresh = new Button
            {
                Text = "🔄 Actualizar Logs",
                Size = new Size(150, 40),
                Location = new Point(15, 55),
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefreshLogs_Click;

            // ListView
            var listView = new ListView
            {
                Name = "lvLogs",
                Location = new Point(15, 105),
                Size = new Size(panel.Width - 30, panel.Height - 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F)
            };

            listView.Columns.Add("Fecha/Hora", 130);
            listView.Columns.Add("Base de Datos", 150);
            listView.Columns.Add("Estado", 100);
            listView.Columns.Add("Duración", 100);
            listView.Columns.Add("Tamaño", 100);
            listView.Columns.Add("Mensaje", 300);

            panel.Controls.AddRange(new Control[] { lblTitle, btnRefresh, listView });
            tabLogs.Controls.Add(panel);
        }

        // Event Handlers
        private async void BtnAddDatabase_Click(object? sender, EventArgs e)
        {
            var form = new DatabaseConfigForm(_configService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshDatabaseList();
            }
        }

        private async void BtnAddFtp_Click(object? sender, EventArgs e)
        {
            var form = new ClientConfigForm(_configService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshFtpList();
            }
        }

        private async void BtnAddBackup_Click(object? sender, EventArgs e)
        {
            var form = new BackupConfigForm(_configService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshBackupList();
            }
        }

        private async void BtnRefreshLogs_Click(object? sender, EventArgs e)
        {
            await RefreshLogsList();
        }

        // Data Loading Methods
        private async Task LoadAllDataAsync()
        {
            try
            {
                await RefreshDatabaseList();
                await RefreshFtpList();
                await RefreshBackupList();
                await RefreshLogsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshDatabaseList()
        {
            try
            {
                var configs = await _configService.GetDatabaseConfigsAsync();
                var listView = FindListView("lvDatabases");
                
                if (listView != null)
                {
                    listView.Items.Clear();
                    
                    foreach (var config in configs)
                    {
                        var item = new ListViewItem(config.Name);
                        item.SubItems.Add(config.Type.ToString());
                        item.SubItems.Add(config.Server);
                        item.SubItems.Add(config.Database);
                        
                        // Probar estado de conexión
                        try
                        {
                            var form = new DatabaseConfigForm(_configService, config);
                            await form.TestConnectionDirectly(config);
                            item.SubItems.Add("🟢 Conectado");
                            item.BackColor = Color.FromArgb(240, 255, 240); // Verde claro
                        }
                        catch
                        {
                            item.SubItems.Add("🔴 Sin conexión");
                            item.BackColor = Color.FromArgb(255, 240, 240); // Rojo claro
                        }
                        
                        item.Tag = config;
                        listView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando configuraciones de base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshFtpList()
        {
            try
            {
                var configs = await _configService.GetFtpConfigsAsync();
                var listView = FindListView("lvFtp");
                
                if (listView != null)
                {
                    listView.Items.Clear();
                    
                    foreach (var config in configs)
                    {
                        var parts = config.Branch.Split('-');
                        var cliente = parts.Length > 0 ? parts[0] : config.Branch;
                        var sucursal = parts.Length > 1 ? string.Join("-", parts.Skip(1)) : "";
                        
                        var item = new ListViewItem(cliente);
                        item.SubItems.Add(sucursal);
                        item.SubItems.Add(config.Host);
                        item.SubItems.Add(config.Port.ToString());
                        item.SubItems.Add(config.UseSsl ? "Sí" : "No");
                        
                        // Probar estado de conexión FTP
                        try
                        {
                            var form = new ClientConfigForm(_configService, config);
                            await form.TestConnectionDirectly(config);
                            item.SubItems.Add("🟢 Conectado");
                            item.BackColor = Color.FromArgb(240, 255, 240); // Verde claro
                        }
                        catch
                        {
                            item.SubItems.Add("🔴 Sin conexión");
                            item.BackColor = Color.FromArgb(255, 240, 240); // Rojo claro
                        }
                        
                        item.Tag = config;
                        listView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando configuraciones de FTP: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshBackupList()
        {
            try
            {
                var configs = await _configService.GetBackupConfigsAsync();
                var listView = FindListView("lvBackups");
                
                if (listView != null)
                {
                    listView.Items.Clear();
                    
                    foreach (var config in configs)
                    {
                        var item = new ListViewItem(config.Name);
                        item.SubItems.Add(config.DatabaseConfigName);
                        item.SubItems.Add(config.FtpConfigName);
                        
                        // Formatear horario desde cron expression
                        var scheduleText = FormatCronExpression(config.Schedule);
                        item.SubItems.Add(scheduleText);
                        
                        // Calcular próximo backup
                        var nextBackup = CalculateNextBackup(config.Schedule);
                        item.SubItems.Add(nextBackup);
                        
                        // Estado con colores
                        var statusText = config.IsEnabled ? "🟢 Activo" : "🔴 Inactivo";
                        item.SubItems.Add(statusText);
                        
                        // Color de fondo según estado
                        if (config.IsEnabled)
                        {
                            item.BackColor = Color.FromArgb(240, 255, 240); // Verde claro
                        }
                        else
                        {
                            item.BackColor = Color.FromArgb(255, 240, 240); // Rojo claro
                        }
                        
                        item.Tag = config;
                        listView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando configuraciones de backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshLogsList()
        {
            try
            {
                var logs = await _logService.GetLogsAsync(50);
                var listView = FindListView("lvLogs");
                
                if (listView != null)
                {
                    listView.Items.Clear();
                    
                    foreach (var log in logs)
                    {
                        var item = new ListViewItem(log.StartTime.ToString("yyyy-MM-dd HH:mm"));
                        item.SubItems.Add(log.DatabaseName);
                        item.SubItems.Add(log.Status.ToString());
                        item.SubItems.Add(log.Duration?.ToString(@"hh\:mm\:ss") ?? "-");
                        item.SubItems.Add(FormatFileSize(log.BackupSizeBytes));
                        item.SubItems.Add(log.Message);
                        
                        listView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ListView? FindListView(string name)
        {
            return tabControl?.Controls.Cast<TabPage>()
                .SelectMany(tab => tab.Controls.Cast<Control>())
                .SelectMany(control => control.Controls.Cast<Control>())
                .OfType<ListView>()
                .FirstOrDefault(lv => lv.Name == name);
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes == 0) return "-";
            
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            
            return $"{number:n1} {suffixes[counter]}";
        }

        private static string FormatCronExpression(string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
                return "-";

            try
            {
                var parts = cronExpression.Split(' ');
                if (parts.Length >= 2)
                {
                    var minute = parts[0];
                    var hour = parts[1];
                    return $"Diario {hour}:{minute}";
                }
            }
            catch
            {
                // Si hay error parseando, mostrar el cron tal como está
            }

            return cronExpression;
        }

        private static string CalculateNextBackup(string cronExpression)
        {
            if (string.IsNullOrEmpty(cronExpression))
                return "-";

            try
            {
                var parts = cronExpression.Split(' ');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[0], out int minute) && int.TryParse(parts[1], out int hour))
                    {
                        var now = DateTime.Now;
                        var next = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                        
                        // Si ya pasó la hora de hoy, programar para mañana
                        if (next <= now)
                        {
                            next = next.AddDays(1);
                        }
                        
                        return next.ToString("dd/MM HH:mm");
                    }
                }
            }
            catch
            {
                // Si hay error calculando, mostrar mensaje genérico
            }

            return "-";
        }

        // Métodos para el menú contextual de bases de datos
        private async Task EditDatabase_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para editar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as DatabaseConfig;
            if (config == null) return;
            
            var form = new DatabaseConfigForm(_configService, config);
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshDatabaseList();
            }
        }

        private async Task DeleteDatabase_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para eliminar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as DatabaseConfig;
            if (config == null) return;
            
            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la configuración '{config.Name}'?\n\nEsta acción no se puede deshacer.", 
                "Confirmar Eliminación", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _configService.DeleteDatabaseConfigAsync(config.Id);
                    await RefreshDatabaseList();
                    MessageBox.Show("Configuración eliminada correctamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error eliminando configuración: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task TestDatabase_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para probar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as DatabaseConfig;
            if (config == null) return;
            
            try
            {
                // Crear el formulario para hacer la prueba
                var form = new DatabaseConfigForm(_configService, config);
                await form.TestConnectionDirectly(config);
                
                MessageBox.Show($"✅ Conexión exitosa a '{config.Name}'!", "Prueba de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error en la conexión a '{config.Name}':\n{ex.Message}", 
                    "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Métodos para el menú contextual de FTP/Clientes
        private async Task EditFtp_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para editar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as FtpConfig;
            if (config == null) return;
            
            var form = new ClientConfigForm(_configService, config);
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshFtpList();
            }
        }

        private async Task DeleteFtp_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para eliminar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as FtpConfig;
            if (config == null) return;
            
            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la configuración '{config.Name}'?\n\nEsta acción no se puede deshacer.", 
                "Confirmar Eliminación", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _configService.DeleteFtpConfigAsync(config.Id);
                    await RefreshFtpList();
                    MessageBox.Show("Configuración eliminada correctamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error eliminando configuración: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task TestFtp_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para probar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as FtpConfig;
            if (config == null) return;
            
            try
            {
                // Crear el formulario para hacer la prueba
                var form = new ClientConfigForm(_configService, config);
                await form.TestConnectionDirectly(config);
                
                MessageBox.Show($"✅ Conexión FTP exitosa a '{config.Name}'!\n\nEstructura de carpetas verificada:\n{config.BasePath}/{config.Branch}/backups/", "Prueba de Conexión", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error en la conexión FTP a '{config.Name}':\n{ex.Message}", 
                    "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Métodos para el menú contextual de configuraciones de backup
        private async Task EditBackup_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para editar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as BackupConfig;
            if (config == null) return;
            
            var form = new BackupConfigForm(_configService, config);
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                await RefreshBackupList();
            }
        }

        private async Task DeleteBackup_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para eliminar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as BackupConfig;
            if (config == null) return;
            
            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar la configuración '{config.Name}'?\n\nEsta acción no se puede deshacer.", 
                "Confirmar Eliminación", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _configService.DeleteBackupConfigAsync(config.Id);
                    await RefreshBackupList();
                    MessageBox.Show("Configuración eliminada correctamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error eliminando configuración: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ToggleBackup_Click(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecciona una configuración para activar/desactivar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var config = listView.SelectedItems[0].Tag as BackupConfig;
            if (config == null) return;
            
            try
            {
                config.IsEnabled = !config.IsEnabled;
                config.UpdatedAt = DateTime.Now;
                
                await _configService.UpdateBackupConfigAsync(config);
                await RefreshBackupList();
                
                var status = config.IsEnabled ? "activada" : "desactivada";
                MessageBox.Show($"Configuración '{config.Name}' {status} correctamente.", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cambiando estado de configuración: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
