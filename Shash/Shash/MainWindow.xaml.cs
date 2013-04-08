using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shash
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string plaintext = "";
        public string loadedFilename = "(no file)";

        const string SHA1 = "SHA-1";
        const string SHA256 = "SHA-256";
        const string SHA512 = "SHA-512";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdatePlainText(object sender, RoutedEventArgs e)
        {
            plaintext = PlaintextTextBox.Text;
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            try
            {
                // Instantiate open file dialog
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "";
                dlg.DefaultExt = "";
                dlg.Filter = "All files (*.*)|*.*";
                dlg.Title = "Load Plaintext File";

                // Show dialog
                Nullable<bool> result = dlg.ShowDialog();

                // Process choice
                if (result == true)
                {
                    loadedFilename = dlg.FileName;

                    FileStream handle = File.OpenRead(loadedFilename);
                    StreamReader reader = new StreamReader(handle);

                    plaintext = reader.ReadToEnd();

                    handle.Close();

                    string filenameOnly = System.IO.Path.GetFileName(loadedFilename);
                    PlaintextTextBox.Text = "(loaded from " + filenameOnly + ")";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySHA", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerifyFile(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loadedFilename.Length == 0)
                {
                    LoadFile(sender, e);
                }

                // Plaintext is loaded. Now load the hash file

                // Instantiate open file dialog
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = loadedFilename;
                dlg.DefaultExt = ".sha1";
                dlg.Filter = "Hash files (*.sha1)|*.sha1";
                dlg.Title = "Load Hash File";

                // Show dialog
                Nullable<bool> result = dlg.ShowDialog();

                string retrievedHash = "";

                // Process choice
                if (result == true)
                {
                    loadedFilename = dlg.FileName;

                    FileStream handle = File.OpenRead(loadedFilename);
                    StreamReader reader = new StreamReader(handle);

                    retrievedHash = reader.ReadLine(); // Only read the first line

                    handle.Close();

                    HashTextBox.Text = retrievedHash;
                }

                // Assume validity of hash
                string generatedHash = GenerateHash(plaintext);

                bool isValid = (retrievedHash == generatedHash);

                if (isValid)
                {
                    MessageBox.Show("Message digest matched.", "MySHA", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Message digest did not match.", "MySHA", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySHA", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateHash(string plaintext)
        {
            return "<hash>" + plaintext.Length.ToString();
        }

        private void GenerateAndDisplayHash(object sender, RoutedEventArgs e)
        {
            // Do magic with plaintext (not PlainTextBox.Text)
            string hashMode = HashModeComboBox.Text;
            HashTextBox.Text = GenerateHash(plaintext);
        }

        private void SaveHashToFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string extension = "sha1";

                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = "Hash files (*." + extension + ")|*." + extension;

                string hash = HashTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Shash", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
