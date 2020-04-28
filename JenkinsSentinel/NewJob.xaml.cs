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
    /// Interaction logic for NewJob.xaml
    /// </summary>
    public partial class NewJob : Window
    {
        public NewJob(ListBox Jobs, Sentinel JenkinsSentinel)
        {
            InitializeComponent();
            this.persistor = Persistor.GetInstance();
            this.jobs = Jobs;
            this.sentinel = JenkinsSentinel;
        }

        private ListBox jobs;
        private Sentinel sentinel;
        private Persistor persistor;

        private void btn_add_new_job_Click(object sender, RoutedEventArgs e)
        {
            string jobName = txt_new_job_name.Text;
            string jobUrl = txt_new_job_url.Text;
            bool temporary = check_temporary_job.IsChecked.Value;
            if (jobName == String.Empty) jobName = jobUrl;


            JenkinsJob newJob = new JenkinsJob(jobName, jobUrl, temporary, sentinel.LastIndex + 1);
            newJob.NotifySettings = new NotifySettings
            {
                NotifyWhenJobBecomesGreen = check_notify_green.IsChecked.Value,
                NotifyWhenJobBecomesRed = check_notify_red.IsChecked.Value,
                NotifyWhenJobStateChanges = check_notify_status_changes.IsChecked.Value,
                NotifyWhenBuildIsComplete = check_notify_build_completed.IsChecked.Value
            };
            if (temporary) newJob.RemoveIfCompleted = check_remove_done_job.IsChecked.Value;

            sentinel.AddNewJob(newJob);
        }

        private bool ValidateInput(string JobName, string JobUrl)
        {
            if (JobUrl == String.Empty) return false;
            return true;
        }

        private void txt_new_job_name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_new_job_url_TextChanged(object sender, TextChangedEventArgs e)
        {
            TryParseJobName(txt_new_job_url.Text);
            if (!ValidateInput(txt_new_job_name.Text, txt_new_job_url.Text)) btn_add_new_job.IsEnabled = false;
            else btn_add_new_job.IsEnabled = true;
        }

        private void TryParseJobName(string Url)
        {
            if (Url == String.Empty) return;
            string[] parts = Url.Split('/');
            List<string> nameParts = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "job" && i < parts.Length - 1 && parts[i + 1] != "CloudBand" && parts[i + 1] != "CBAM") nameParts.Add(parts[i + 1]);
            }
            txt_new_job_name.Text = String.Join(" ", nameParts);
        }

        private void check_temporary_job_Checked(object sender, RoutedEventArgs e)
        {
            check_remove_done_job.IsEnabled = true;
            check_remove_done_job.IsChecked = true;
        }

        private void check_temporary_job_Unchecked(object sender, RoutedEventArgs e)
        {
            check_remove_done_job.IsEnabled = false;
            check_remove_done_job.IsChecked = false;
        }
    }
}
