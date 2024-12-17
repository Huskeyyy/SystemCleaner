using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SystemCleaner
{
    class Program
    {
        static void Main(string[] args)
        {

            if (!IsAdministrator())
            {
                Console.WriteLine("Please run as an administrator.");
                return;
            }

            PCCleaner cleaner = new PCCleaner();

            while (true)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        cleaner.ClearDNSCache();
                        break;
                    case "2":
                        cleaner.ClearBrowserCache();
                        break;
                    case "3":
                        cleaner.ClearWindowsUpdateCache();
                        break;
                    case "4":
                        cleaner.ClearRecycleBin();
                        break;
                    case "5":
                        cleaner.RunDiskCleanup();
                        break;
                    case "6":
                        cleaner.ClearThumbnailCache();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("SystemCleaner");
            Console.WriteLine("------------------");
            Console.WriteLine("1. Clean DNS Cache");
            Console.WriteLine("2. Clear Browser Cache");
            Console.WriteLine("3. Clear Windows Update Cache");
            Console.WriteLine("4. Empty Recycle Bin");
            Console.WriteLine("5. Run Disk Cleanup");
            Console.WriteLine("6. Clear Thumbnail Cache");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
        }

        static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }

    class PCCleaner
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        // Flags for SHEmptyRecycleBin
        const uint SHERB_NOCONFIRMATION = 0x00000001; // No empty confirmation
        const uint SHERB_NOPROGRESSUI = 0x00000002; // No progress tracking
        const uint SHERB_NOSOUND = 0x00000004; // No sound on completion

        public void CleanTempFiles()
        {
            try
            {
                string windowsTempPath = Path.GetTempPath();
                DeleteFilesInDirectory(windowsTempPath);

                string userTempPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "AppData", "Local", "Temp");
                DeleteFilesInDirectory(userTempPath);

                Console.WriteLine("Temporary files cleaned successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning temp files: {ex.Message}");
            }
        }

        public void ClearBrowserCache()
        {
            try
            {
                // Chrome
                string chromeCache = Path.Combine(
                    Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                    @"Google\Chrome\User Data\Default\Cache"
                );
                DeleteFilesInDirectory(chromeCache);

                // Firefox
                string firefoxProfile = Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    @"Mozilla\Firefox\Profiles"
                );
                DeleteFilesInDirectory(firefoxProfile);

                // Edge
                string edgeCache = Path.Combine(
                    Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                    @"Microsoft\Edge\User Data\Default\Cache"
                );
                DeleteFilesInDirectory(edgeCache);

                // Brave
                string braveCaceh = Path.Combine(
                    Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                    @"BraveSoftware\Brave-Browser\User Data"
                );
                Console.WriteLine("Browser caches cleared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing browser caches: {ex.Message}");
            }
        }

        public void ClearWindowsUpdateCache()
        {
            try
            {
                // Stops the Windows Update service
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = "stop wuauserv",
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }

                // Clears the  Windows Update cache
                string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string updateCache = Path.Combine(windowsDirectory, @"SoftwareDistribution");
                DeleteFilesInDirectory(updateCache);

                // Restarts the Windows Update service
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = "start wuauserv",
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }

                Console.WriteLine("Windows Update cache cleared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing Windows Update cache: {ex.Message}");
            }
        }

        public void ClearRecycleBin()
        {
            try
            {
                // Empties the Recycle Bin with the Windows Shell API using the sound, conf and noprogress flags
                uint result = SHEmptyRecycleBin(IntPtr.Zero, null,
                    SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);

                if (result == 0)
                {
                    Console.WriteLine("Recycle Bin emptied successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to empty Recycle Bin.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error emptying Recycle Bin: {ex.Message}");
            }
        }

        public void RunDiskCleanup()
        {
            try
            {
                Process.Start("cleanmgr.exe");
                Console.WriteLine("Disk Cleanup utility launched.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Disk Cleanup: {ex.Message}");
            }
        }

        public void ClearThumbnailCache()
        {
            try
            {
                string thumbnailCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Windows\Explorer");

                var thumbnailFiles = Directory.GetFiles(thumbnailCache, "thumbcache*.db");

                foreach (var file in thumbnailFiles)
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted thumbnail cache file: {file}");
                }

                Console.WriteLine("Thumbnail cache has been cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing thumbnail cache: " + ex.Message);
            }

        }

        public void ClearDNSCache()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = "ipconfig",            
                    Arguments = "/flushdns",          // Argument to flush DNS
                    UseShellExecute = false,          // Shell not needed for this command
                    CreateNoWindow = true,            // Hides the command window
                };

                using (Process process = Process.Start(processStartInfo))
                {
                    process.WaitForExit();

                    Console.WriteLine("DNS Cache Cleared.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing DNS cache: " + ex.Message);
            }
        }

        public void DeleteFilesInDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath)) return;

            try
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);

                foreach (FileInfo file in directory.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Error deleting file {file.FullName}: Unauthorized access. {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error deleting file {file.FullName}: IO error. {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error deleting file {file.FullName}: {ex.Message}");
                    }
                }

                // Deletes all subdirectories
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true); 
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Error deleting directory {dir.FullName}: Unauthorized access. {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error deleting directory {dir.FullName}: IO error. {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error deleting directory {dir.FullName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing directory {directoryPath}: {ex.Message}");
            }
        }
    }
}