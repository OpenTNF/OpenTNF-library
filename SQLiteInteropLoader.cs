using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenTNF.Library
{
    internal static class SQLiteInteropLoader
    {
        private const string SQLiteInteropModuleName = "SQLite.Interop.dll";
        private const string SQLiteInteropLocationKey = "SQLiteInteropLocation";

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetDllDirectory(int nBufferLength, StringBuilder lpPathName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        internal static void LoadSQLiteInterop()
        {
            if (!LoadSQLiteInteropPrivate())
            {
                Console.WriteLine($"Retry after SetDllDirectory(null)");
                SetDllDirectory(null);
                LoadSQLiteInteropPrivate();
            }
        }
        /// <summary>
        /// Load the appropriate SQLite.Interop.dll for 32/64-bit processes.
        /// </summary>
        internal static bool LoadSQLiteInteropPrivate()
        {
            // If the module is already loaded, return
            if (ModuleIsLoaded(SQLiteInteropModuleName))
            {
                return true;
            }

            // Try to load the module from the process directory
            IntPtr loadResult = LoadLibrary(SQLiteInteropModuleName);
            if (loadResult != IntPtr.Zero)
            {
                return true;
            }

            // Check for configured DLL location
            string path = ConfigurationManager.AppSettings[SQLiteInteropLocationKey];

            if (path != null) // If a location is configured, check that it exists and use it.
            {
                if (!Directory.Exists(path))
                {
                    throw new OpenTnfException(string.Format("Unable to find configured path '{0}' for {1} defined in config key '{2}'.",
                        path, SQLiteInteropModuleName, SQLiteInteropLocationKey));
                }
            }
            else // Use default DLL locations depending on processor architecture
            {
                bool is64Bit = IntPtr.Size == 8;
                if (is64Bit)
                {
                    path = @"dlls\x64\";
                }
                else
                {
                    path = @"dlls\x86\";
                }
            }

            // Load DLL from file location
            string fileName = Path.Combine(path, SQLiteInteropModuleName);
            loadResult = LoadLibrary(fileName);
            if (loadResult == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load 'SQLite.Interop.dll' at path: '{fileName}' " +
                                  $"(Current directory: {Environment.CurrentDirectory}. " +
                                  $"File exists: {(File.Exists(fileName) ? "Yes": "No")}. " +
                                  $"Error code: {Marshal.GetLastWin32Error()})");
                return false;
            }
            return true;
        }

        private static bool ModuleIsLoaded(string moduleName)
        {
            Process process = Process.GetCurrentProcess();

            ProcessModuleCollection modules = process.Modules;
            foreach (ProcessModule processModule in modules)
            {
                if (string.Equals(moduleName, processModule.ModuleName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
