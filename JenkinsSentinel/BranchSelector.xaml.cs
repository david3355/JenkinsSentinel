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
using JenkinsSentinel.src.jenkinsinput;
using JenkinsSentinel.src;

namespace JenkinsSentinel
{
    /// <summary>
    /// Interaction logic for BranchSelector.xaml
    /// </summary>
    public partial class BranchSelector : Window
    {
        public BranchSelector(JenkinsTemplate Template, Sentinel Sentinel)
        {
            InitializeComponent();
            this.template = Template;
            this.sentinel = Sentinel;
            list_branches.ItemsSource = new List<string> { "master", "19.5.2rc", "19.5.1rc" };
            list_branches.SelectedIndex = 0;
        }

        JenkinsTemplate template;
        Sentinel sentinel;

        private void btn_select_branch_Click(object sender, RoutedEventArgs e)
        {
            if (list_branches.SelectedIndex != -1)
            {
                template.Branch = list_branches.SelectedItem.ToString();
                JobTemplateEditor templateEditor = new JobTemplateEditor(template, sentinel);
                templateEditor.Show();
                this.Close();
            }
        }
    }
}
