﻿using System;
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

        private const string DefaultOrbBasicProgramFilePath = "orbbasic.txt";

        private SpheroConnector connector = new SpheroConnector();
        private Sphero sphero = null;

        public MainWindow()
        {
            InitializeComponent();
            FindDevices();
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

        #region orbBasic

        private void LoadDefaultCodeFromFile()
        {
            Code.Text = File.ReadAllText(DefaultOrbBasicProgramFilePath, Encoding.UTF8);
        }

        private void RunCode(object sender, RoutedEventArgs e)
        {
            if (sphero == null)
            {
                MessageBox.Show("No Sphero connected.");
                return;
            }

            var area = StorageArea.Temporary;
            IEnumerable<string> programLines = GetOrbBasicLines();
            foreach (var programLine in programLines)
            {
                Console.WriteLine(programLine);
            }
            sphero.EraseOrbBasicStorage(area);
            sphero.SendOrbBasicProgram(area, programLines);

            // TODO: Split this into separate button?
            sphero.ExecuteOrbBasicProgram(area, 10);
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
