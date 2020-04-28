using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JenkinsSentinel.src;

namespace JenkinsSentinel
{
    public delegate void SaveSettings();

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(Sentinel JenkinsSentinel, SaveSettings SaveCallback)
        {
            InitializeComponent();
            this.sentinel = JenkinsSentinel;
            this.saveCallback = SaveCallback;
            LoadCurrentSettings();
        }

        private Sentinel sentinel;
        private SaveSettings saveCallback;

        private void LoadCurrentSettings()
        {
            txt_default_cloud.Text = sentinel.DefaultCloud;
            txt_check_period.Text = sentinel.InspectFrequency.ToString();
            check_topmost.IsChecked = sentinel.WindowTopmost;
        }

        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            sentinel.DefaultCloud = txt_default_cloud.Text;
            sentinel.InspectFrequency = int.Parse(txt_check_period.Text);
            sentinel.WindowTopmost = check_topmost.IsChecked.Value;
            saveCallback();

            MessageBoxResult res = MessageBox.Show("Settings saved");
            if (res == MessageBoxResult.OK) this.Close();
        }
    }
}
