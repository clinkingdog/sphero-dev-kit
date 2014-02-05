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
using SpheroNET;

namespace orbBasicUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        #region Setup

        private const string DefaultCodeFileLocation = "C:\\Users\\Nick\\dev\\sphero-dev-kit\\samples\\";
        private const string DefaultCodeFileName = "colour-shuffle";
        private const StorageArea storageArea = StorageArea.Temporary;

        private SpheroConnector connector = new SpheroConnector();
        private Sphero sphero = null;

        public MainWindow()
        {
            InitializeComponent();
            FindDevices();
            FindCodeFiles();
            LoadDefaultCodeFromFile();
            SetColourPanelBackground();
        }

        public void Dispose()
        {
            connector.Close();
        }

        #endregion

        #region Connection

        private void FindDevices(object sender, RoutedEventArgs e)
        {
            FindDevices();
        }

        private void FindDevices()
        {
            Devices.Items.Clear();
            connector.Scan();
            var deviceNames = connector.DeviceNames;
            for (int i = 0; i < deviceNames.Count; i++)
            {
                Devices.Items.Add(String.Format("{0}: {1}", i, deviceNames[i]));
            }
        }

        private void Devices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConnectToSelectedDevice();
        }

        private void ConnectToSelectedDevice()
        {
            var selectedIndex = Devices.SelectedIndex;

            if (selectedIndex == -1)
            {
                return;
            }

            try
            {
                Window.Cursor = Cursors.Wait;
                // TODO: Disable all controls

                sphero = connector.Connect(selectedIndex);

                Background.Background = new SolidColorBrush(Color.FromRgb(0x66,0xCC,0xEC));
                CodePanel.Fill = new SolidColorBrush(Color.FromRgb(0x66, 0xCC, 0xEC));
                CodePanelTitle.Background = new SolidColorBrush(Color.FromRgb(0x66, 0xCC, 0xEC));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
            finally
            {
                Window.Cursor = Cursors.Arrow;
                // TODO: Re-enable all controls
            }
        }

        #endregion

        #region Colour

        private void SetSelectedColor(object sender, RoutedEventArgs e)
        {
            if (sphero == null)
            {
                MessageBox.Show("No Sphero connected.");
                return;
            }

            byte R, G, B;

            try
            {
                R = byte.Parse(R_value.Text);
                G = byte.Parse(G_value.Text);
                B = byte.Parse(B_value.Text);
            }
            catch
            {
                MessageBox.Show("Yeah, or you could try actual byte-size numbers, shit-for-brains.");
                return;
            }

            sphero.SetRGBLEDOutput(R, G, B);
        }

        private void RGB_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColourPanelBackground();
        }

        private void SetColourPanelBackground()
        {
            byte R, G, B;

            try
            {
                R = byte.Parse(R_value.Text);
                G = byte.Parse(G_value.Text);
                B = byte.Parse(B_value.Text);
            }
            catch
            {
                return;
            }

            ColourPanel.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
        }

        private void SelectAllText(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        #endregion

        #region Code File Management

        private void FindCodeFiles(object sender, RoutedEventArgs e)
        {
            FindCodeFiles();
        }

        private void FindCodeFiles()
        {
            SampleCodeFiles.Items.Clear();

            foreach (string filePath in Directory.EnumerateFiles(DefaultCodeFileLocation))
            {
                SampleCodeFiles.Items.Add(filePath.Substring(filePath.LastIndexOf('\\') + 1).Replace(".txt", ""));
            }
        }

        private void SampleCodeFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedCodeFile();
        }

        private void LoadDefaultCodeFromFile()
        {
            LoadCodeFile(DefaultCodeFileName);
        }

        private void LoadSelectedCodeFile()
        {
            if (SampleCodeFiles.SelectedItem == null)
            {
                return;
            }

            var filename = SampleCodeFiles.SelectedItem.ToString();
            LoadCodeFile(filename);
        }

        private void LoadCodeFile(string filename)
        {
            Code.Text = File.ReadAllText(DefaultCodeFileLocation + filename + ".txt");
            CodeFileName.Text = filename;
        }

        private void ClearCode_Click(object sender, RoutedEventArgs e)
        {
            CodeFileName.Text = string.Empty;
            Code.Text = string.Empty;
        }

        private void SaveCode_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentCode();
        }

        private void SaveCurrentCode()
        {
            var filename = CodeFileName.Text;

            if (filename == String.Empty)
            {
                return;
            }

            var filePath = DefaultCodeFileLocation + CodeFileName.Text + ".txt";

            if (File.Exists(filePath))
            {
                var result = MessageBox.Show("File exists. Overwrite?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            File.WriteAllText(filePath, Code.Text);
        }

        #endregion

        #region orbBasic

        private void RunCode(object sender, RoutedEventArgs e)
        {
            if (sphero == null)
            {
                MessageBox.Show("No Sphero connected.");
                return;
            }

            IEnumerable<string> programLines = GetOrbBasicLines();

            sphero.EraseOrbBasicStorage(storageArea);
            sphero.SendOrbBasicProgram(storageArea, programLines);
            sphero.ExecuteOrbBasicProgram(storageArea, 10);
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            sphero.AbortOrbBasicProgram();
        }

        private IEnumerable<string> GetOrbBasicLines()
        {
            var rawLines = Code.Text.Split('\n');
            var result = new List<string>();
            foreach (var rawLine in rawLines)
            {
                if (!string.IsNullOrEmpty(rawLine) && rawLine[0] != '\'')
                {
                    var line = rawLine;

                    if (!line.EndsWith("\r"))
                    {
                        line += '\r';
                    }

                    result.Add(line);
                }
            }
            return result;
        }

        #endregion
    }
}
