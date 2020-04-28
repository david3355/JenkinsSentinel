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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JenkinsSentinel.src;
using System.IO;
using System.ComponentModel;
using System.Windows.Threading;
using JenkinsSentinel.src.jenkinsinput;

namespace JenkinsSentinel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISentinelEvents
    {
        public MainWindow()
        {
            InitializeComponent();
            persistor = Persistor.GetInstance();

            if (File.Exists(Persistor.CredentialsFileName))
            {
                JenkinsCredentials creds = persistor.ReadCredentials();
                txt_username.Text = creds.Username;
                txt_passkey.Text = creds.Password;
            }

            sentinel = new Sentinel(CHECK_PERIOD, this);
            if (File.Exists(Persistor.SentinelFileName))
            {
                Sentinel sentinelSave = persistor.ReadJobs();
                sentinel.Jobs = sentinelSave.Jobs;
                sentinel.LastIndex = sentinelSave.LastIndex;
                sentinel.WindowTopmost = sentinelSave.WindowTopmost;
                sentinel.DefaultCloud = sentinelSave.DefaultCloud;
                sentinel.InspectFrequency = sentinelSave.InspectFrequency;
            }

            this.Topmost = sentinel.WindowTopmost;
            sentinel.AddReportListener(ShowNotification);
        }

        private Sentinel sentinel;
        private Persistor persistor;
        private const int CHECK_PERIOD = 15000;
        private DateTime lastChecked;
        private string CurrentUser = String.Empty;

        private void ShowNotification(JenkinsJob UpdatedJob)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Notification notif = new Notification(UpdatedJob);
                notif.Show();
            }));
        }

        private void RefreshJobList()
        {
            list_jobs.Items.Refresh();
        }

        private void SortJobList(ListSortDirection Direction = ListSortDirection.Ascending)
        {

            list_jobs.Items.SortDescriptions.Clear();
            list_jobs.Items.SortDescriptions.Add(new SortDescription("Index", Direction));
            RefreshJobList();
        }

        private void job_name_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                JobEditor editor = new JobEditor(list_jobs, sentinel, job);
                editor.Show();
            }

        }

        private void menu_add_new_job_Click(object sender, RoutedEventArgs e)
        {
            NewJob njWindow = new NewJob(list_jobs, sentinel);
            njWindow.Show();
        }

        private void img_delete_job_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to remove this job?", "Remove Job", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) RemoveJob();
        }

        private void img_refresh_job_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                sentinel.UpdateJob(job);
            }
        }

        private void RemoveJob()
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                sentinel.RemoveJob(job);
            }
        }

        private bool AreLoginCredsValid(string Username, string Password)
        {
            return Username != String.Empty && Password != String.Empty;
        }

        private bool AreLoginCredsAuthorized(string Username, string Password)
        {
            return new JenkinsClient(new JenkinsCredentials(Username, Password), this).Authenticate();
        }

        private void StoreCredentials(JenkinsCredentials Creds)
        {
            persistor.PersistCredentials(Creds);
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            string username = txt_username.Text;
            string passkey = txt_passkey.Text;
            if (AreLoginCredsAuthorized(username, passkey))
            {
                CurrentUser = username;
                JenkinsCredentials creds = new JenkinsCredentials(username, passkey);
                sentinel.SetCredentials(creds);
                this.Title = String.Format("Jenkins Sentinel - [{0}]", username);
                LoadAndUpdateAllJobs(false);
                sentinel.StartWatching();
                if (check_save_login.IsChecked.Value) StoreCredentials(creds);

                panel_login.Visibility = Visibility.Collapsed;
                panel_jobs.Visibility = Visibility.Visible;
                menu_jobs.IsEnabled = true;
                menu_templates.IsEnabled = true;
            }
            else MessageBox.Show("Login credentials are not valid!");
        }

        private void LoadAndUpdateAllJobs(bool Notify)
        {
            foreach (JenkinsJob job in sentinel.Jobs)
            {
                list_jobs.Items.Add(job);
                sentinel.UpdateJob(job, Notify);
            }
            SortJobList();
        }


        private void txt_username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AreLoginCredsValid(txt_username.Text, txt_passkey.Text)) btn_login.IsEnabled = true;
            else btn_login.IsEnabled = false;
        }

        private void txt_passkey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AreLoginCredsValid(txt_username.Text, txt_passkey.Text)) btn_login.IsEnabled = true;
            else btn_login.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sentinel != null) sentinel.StopWatching();
        }

        private void img_open_job_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                System.Diagnostics.Process.Start(job.JobUrl);
            }

        }

        private void menu_product_info_Click(object sender, RoutedEventArgs e)
        {
            InfoTemplate productInfo = new InfoTemplate("Developer: David Bakos");
            productInfo.Show();
        }

        private void menu_get_token_Click(object sender, RoutedEventArgs e)
        {
            PasskeyInfo info = new PasskeyInfo();
            info.Show();
        }


        public void ConnectionError(string ErrorMessage, string ErrorDetails)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                text_error.Visibility = Visibility.Visible;
                text_error.Text = ErrorMessage;
                text_error.ToolTip = ErrorDetails;
            }));
        }


        public void ConnectionWorks()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                text_error.Visibility = Visibility.Collapsed;
                text_error.Text = String.Empty;
                text_error.ToolTip = String.Empty;
            }));
        }


        public void JobRemoved(JenkinsJob Job)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                list_jobs.Items.Remove(Job);
                RefreshJobList();
            }));
            persistor.PersistJobs(sentinel);
        }

        public void JobAdded(JenkinsJob Job)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                list_jobs.Items.Add(Job);
                RefreshJobList();
            }));
            persistor.PersistJobs(sentinel);
            MessageBox.Show("New job added");
        }

        public void CheckCycleFinished(bool Success)
        {
            if (Success) lastChecked = DateTime.Now;
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Success) RefreshJobList();
                text_status.Text = String.Format("{0}", GetLastCheckedText());
            }));
            if (Success) persistor.PersistJobs(sentinel);
        }

        public void CheckCycleExpired()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                text_status.Visibility = Visibility.Visible;
                text_status.Text = String.Format("Checking in progress... {0}", GetLastCheckedText());
            }));
        }

        private string GetLastCheckedText()
        {
            return String.Format("Last updated: {0}", lastChecked.ToString("MM.dd. HH:mm:ss"));
        }

        private void MoveSelectedJobUp()
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                if (sentinel.MoveJobIndexUp(job))
                {
                    persistor.PersistJobs(sentinel);
                    SortJobList();
                }
            }
        }

        private void MoveSelectedJobDown()
        {
            int idx = list_jobs.SelectedIndex;
            if (idx != -1)
            {
                JenkinsJob job = list_jobs.Items[idx] as JenkinsJob;
                if (sentinel.MoveJobIndexDown(job))
                {
                    persistor.PersistJobs(sentinel);
                    SortJobList();
                }
            }
        }

        private void img_move_up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MoveSelectedJobUp();
        }

        private void img_move_down_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MoveSelectedJobDown();
        }

        private void menu_jumpstart_Click(object sender, RoutedEventArgs e)
        {
            BranchSelector branchSelector = new BranchSelector(new Jumpstart(CurrentUser, sentinel.DefaultCloud), sentinel);
            branchSelector.Show();
        }

        private void menu_buildbot_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet!");
        }

        private void menu_itenv_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet!");
        }

        private void menu_e2e_env_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet!");
        }

        private void menu_sentinel_settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(sentinel, SaveSettings);
            settings.Show();
        }

        private void SaveSettings()
        {
            this.Topmost = sentinel.WindowTopmost;
            persistor.PersistJobs(sentinel);
        }
    }
}
