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
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        public Notification(JenkinsJob Job)
        {
            InitializeComponent();
            this.currentJob = Job;
            Init(Job);
        }

        private JenkinsJob currentJob;

        private void Init(JenkinsJob Job)
        {
            label_jobname.Content = Job.JobName;
            label_jobname.ToolTip = Job.JobUrl;
            label_jobstatus.Content = Job.JobStatus;
            label_time.Content = DateTime.Now.ToString("MM.dd. HH:mm:ss");
            border_background.Background = new SolidColorBrush(Job.JobStatusColor);
        }



        private void CloseNoteWarn()
        {
            this.Close();
        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CloseNoteWarn();
        }

        private void border_background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete: CloseNoteWarn(); break;
            }
        }

        private void img_open_job_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(currentJob.JobUrl);
        }
    }
}
