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
    /// Interaction logic for JobEditor.xaml
    /// </summary>
    public partial class JobEditor : Window
    {
        public JobEditor(ListBox Jobs, Sentinel JenkinsSentinel, JenkinsJob Job)
        {
            InitializeComponent();
            this.persistor = Persistor.GetInstance();
            this.jobs = Jobs;
            this.sentinel = JenkinsSentinel;
            this.currentJob = Job;
            LoadJobData(Job);
        }

        private ListBox jobs;
        private Sentinel sentinel;
        private Persistor persistor;
        private JenkinsJob currentJob;

        private void LoadJobData(JenkinsJob Job)
        {
            txt_job_name.Text = Job.JobName;
            txt_job_url.Text = Job.JobUrl;
            check_notify_green.IsChecked = Job.NotifySettings.NotifyWhenJobBecomesGreen;
            check_notify_red.IsChecked = Job.NotifySettings.NotifyWhenJobBecomesRed;
            check_notify_status_changes.IsChecked = Job.NotifySettings.NotifyWhenJobStateChanges;
            check_notify_build_completed.IsChecked = Job.NotifySettings.NotifyWhenBuildIsComplete;
            check_temporary_job.IsChecked = Job.IsTemporary;
            check_remove_done_job.IsChecked = Job.RemoveIfCompleted;
            check_remove_done_job.IsEnabled = check_temporary_job.IsChecked.Value;
            txt_job_status.Content = Job.JobStatus;
            job_state_color.Fill = new SolidColorBrush(Job.JobStatusColor);
        }

        private void btn_save_job_Click(object sender, RoutedEventArgs e)
        {
            string jobName = txt_job_name.Text;
            if (jobName == String.Empty) jobName = currentJob.JobUrl;
            currentJob.RemoveIfCompleted = check_remove_done_job.IsChecked.Value;
            
            currentJob.NotifySettings = new NotifySettings
            {
                NotifyWhenJobBecomesGreen = check_notify_green.IsChecked.Value,
                NotifyWhenJobBecomesRed = check_notify_red.IsChecked.Value,
                NotifyWhenJobStateChanges = check_notify_status_changes.IsChecked.Value,
                NotifyWhenBuildIsComplete = check_notify_build_completed.IsChecked.Value
            };
            persistor.PersistJobs(sentinel);
            jobs.Items.Refresh();
            this.Close();
        }

    }
}
