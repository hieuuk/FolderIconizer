using System.IO;
using System.Runtime.InteropServices;

namespace FolderIconizer
{
    public class FolderIconUpdater
    {
        // P/Invoke to refresh the folder icon
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);


        public static void ChangeFolderIcon(string folderPath, string iconPath)
        {
            string desktopIniPath = Path.Combine(folderPath, "desktop.ini");

            // Set folder and desktop.ini attributes
            File.SetAttributes(folderPath, File.GetAttributes(folderPath) | FileAttributes.System);
            if (File.Exists(desktopIniPath))
            {
                File.SetAttributes(desktopIniPath, FileAttributes.Normal);
            }

            // Write icon settings to desktop.ini
            using (StreamWriter writer = new StreamWriter(desktopIniPath, false))
            {
                writer.WriteLine("[.ShellClassInfo]");
                writer.WriteLine($"IconResource={iconPath},0");
                writer.Flush();
            }

            // Set desktop.ini to hidden
            File.SetAttributes(desktopIniPath, FileAttributes.Hidden | FileAttributes.System);

            // Refresh the icon
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
