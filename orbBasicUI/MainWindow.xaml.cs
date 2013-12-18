using System;
using System.Collections.Generic;
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

        private SpheroConnector connector = new SpheroConnector();
        private Sphero sphero = null;

        public MainWindow()
        {
            InitializeComponent();
            FindDevices();
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
            connector.Scan();
            var deviceNames = connector.DeviceNames;
            for (int i = 0; i < deviceNames.Count; i++)
            {
                Devices.Items.Add(String.Format("{0}: {1}", i, deviceNames[i]));
            }
        }

        private void ConnectToSelectedDevice(object sender, RoutedEventArgs e)
        {
            var selectedIndex = Devices.SelectedIndex;

            if (selectedIndex == -1)
            {
                return;
            }

            try
            {
                sphero = connector.Connect(selectedIndex);
                MessageBox.Show("Connected, yay!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
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

        private void R_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColourPanelBackground();
        }

        private void G_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetColourPanelBackground();
        }

        private void B_value_TextChanged(object sender, TextChangedEventArgs e)
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

        #endregion
    }
}
