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
using Newtonsoft.Json;
using JenkinsSentinel.src;

namespace JenkinsSentinel
{
    /// <summary>
    /// Interaction logic for JobTemplate.xaml
    /// </summary>
    public partial class JobTemplateEditor : Window
    {
        public JobTemplateEditor(JenkinsTemplate Template, Sentinel Sentinel)
        {
            InitializeComponent();
            this.template = Template;
            this.sentinel = Sentinel;
            LoadTemplate();
        }

        private JenkinsTemplate template;
        private Sentinel sentinel;
        private List<JobEditorParameter> parameters;

        private void LoadTemplate()
        {
            this.Title = template.TemplateName;
            txt_new_job_name.Text = String.Format("{0} {1}", template.Branch, template.TemplateName);

            parameters = template.GetMainParameters();
            foreach (JobEditorParameter p in parameters)
            {
                Grid paramPanel = new Grid();
                paramPanel.ColumnDefinitions.Add(new ColumnDefinition());
                paramPanel.ColumnDefinitions.Add(new ColumnDefinition() );

                
                Label paramName = new Label();
                paramName.Name = String.Format("key_{0}", p.Name);
                SetContentForLabel(paramName, p.Name);
                paramName.FontWeight = FontWeights.Normal;
                paramName.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetColumn(paramName, 0);
                paramPanel.Children.Add(paramName);
                FrameworkElement valueElement = null;
                if (p is TextParameter)
                {
                    TextBox paramValue = new TextBox();
                    paramValue.Name = p.Name;
                    paramValue.Text = (p as TextParameter).Value;
                    valueElement = paramValue;
                }
                else if (p is BooleanParameter)
                {
                    CheckBox paramValue = new CheckBox();
                    paramValue.Name = p.Name;
                    paramValue.IsChecked = (p as BooleanParameter).Value;
                    valueElement = paramValue;
                }
                else if (p is ListParameter)
                {
                    ComboBox paramValue = new ComboBox();
                    paramValue.Name = p.Name;
                    List<string> values = (p as ListParameter).Value;
                    paramValue.ItemsSource = values;
                    if (values.Count > 0) paramValue.SelectedIndex = 0;
                    valueElement = paramValue;
                }

                if (valueElement != null)
                {
                    valueElement.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    paramPanel.Margin = new Thickness(5);
                    Grid.SetColumn(valueElement, 1);
                    paramPanel.Children.Add(valueElement);

                    switch (p.paramType)
                    {
                        case ParamType.MAIN: panel_main_parameters.Children.Add(paramPanel); break;
                        case ParamType.ADVANCED: panel_advanced_parameters.Children.Add(paramPanel); break;
                        case ParamType.NON_EDITABLE: break;
                    }
                }
            }
        }

        private void SetContentForLabel(Label Element, string Value)
        {
            Element.Content = Value.Replace("_", "__");
        }

        private JenkinsInput GetInputParameters()
        {
            JenkinsInput input = new JenkinsInput();
            if (parameters == null) return null;
            foreach (JobEditorParameter p in parameters)
            {
                if (p.paramType == ParamType.NON_EDITABLE) input.Add(p.Name, p.GetValue().ToString()); // Can be only text, number or boolean
                else if (p is TextParameter)
                {
                    TextBox textValue = (TextBox)FindElement(p.Name);
                    panel_main_parameters.FindName(p.Name);
                    input.Add(p.Name, textValue.Text);
                }
                else if (p is BooleanParameter)
                {
                    CheckBox boolValue = (CheckBox)FindElement(p.Name);
                    input.Add(p.Name, boolValue.IsChecked.Value ? "true" : "false");
                }
                else if (p is ListParameter)
                {
                    ComboBox listValue = (ComboBox)FindElement(p.Name);
                    input.Add(p.Name, listValue.SelectedItem.ToString());
                }
            }
            return input;
        }

        private FrameworkElement FindElement(string Name)
        {
            foreach (FrameworkElement Element in panel_main_parameters.Children)
            {
                FrameworkElement valueElement = (Element as Grid).Children[1] as FrameworkElement;
                if (valueElement != null && valueElement.Name == Name) return valueElement;
            }

            foreach (FrameworkElement Element in panel_advanced_parameters.Children)
            {
                FrameworkElement valueElement = (Element as Grid).Children[1] as FrameworkElement;
                if (valueElement != null && valueElement.Name == Name) return valueElement;
            }
            return null;
        }

        private void StartJob()
        {
            JenkinsInput input = GetInputParameters();
            template.SetInput(input);
            try
            {
                sentinel.StartNewJobFromTemplate(template, txt_new_job_name.Text);
                MessageBoxResult result =  MessageBox.Show("New job started successfully", "Job status", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK) this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to start new job. Details: " + e.Message);
            }
        }

        private void btn_start_job_Click(object sender, RoutedEventArgs e)
        {
            StartJob();
        }
    }
}
