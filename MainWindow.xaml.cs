using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Reflection.Metadata.BlobBuilder;

namespace FolderIconizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // P/Invoke to refresh the folder icon
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public MainWindow()
        {
            InitializeComponent();
        }

        void WriteLog(string message)
        {
            tbLogs.Text = $"{DateTime.Now.ToString("HH:mm")}: {message} \n" + tbLogs.Text;
        }

        private async void cbProcess_Click(object sender, RoutedEventArgs e)
        {
            await UpdateFolderIcons(tbFolderPath.Text);
        }


        public async Task UpdateFolderIcons(string parentFolderPath)
        {
            // Ensure the parent folder exists
            if (!Directory.Exists(parentFolderPath))
            {
                System.Windows.MessageBox.Show($"The directory '{parentFolderPath}' was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(cbParentFolder.IsChecked == true)
            {
                string iconFilePath = System.IO.Path.Combine(parentFolderPath, "folder.ico");

                // Check if folder.icon exists
                if (File.Exists(iconFilePath))
                {
                    WriteLog("Updating folder icon for " + parentFolderPath);
                    FolderIconUpdater.ChangeFolderIcon(parentFolderPath, iconFilePath);
                }
            }

            if (cbSubFolders.IsChecked == true)
            {
                // Iterate through each subfolder in the parent folder
                foreach (var folderPath in Directory.GetDirectories(parentFolderPath))
                {
                    string iconFilePath = System.IO.Path.Combine(folderPath, "folder.ico");

                    // Check if folder.icon exists
                    if (File.Exists(iconFilePath))
                    {
                        WriteLog("Updating folder icon for " + folderPath);
                        FolderIconUpdater.ChangeFolderIcon(folderPath, iconFilePath);
                    }
                }
            }

            WriteLog("Done!");
            MessageBox.Show("Done!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}