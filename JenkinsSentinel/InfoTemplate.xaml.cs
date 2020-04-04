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

namespace JenkinsSentinel
{
    /// <summary>
    /// Interaction logic for InfoTemplate.xaml
    /// </summary>
    public partial class InfoTemplate : Window
    {
        public InfoTemplate(string Text)
        {
            InitializeComponent();
            tx_info.Text = Text;
        }

        private void border_infoheader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void img_close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


    }
}
