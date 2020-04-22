using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using JenkinsSentinel.src.jenkinsdata;

namespace JenkinsSentinel.src
{
    [XmlRootAttribute("JenkinsJob", IsNullable = false)]
    public class JenkinsJob: ICloneable
    {
        public JenkinsJob(string JobName, string JobUrl, bool Temporary, int Index)
        {
            this.name = JobName;
            this.url = JobUrl;
            this.id = url;
            this.temporary = Temporary;
            this.removeIfCompleted = false;
            this.notifySettings = new NotifySettings();
            this.index = Index;
            this.category = DEFAULT_CATEGORY;
        }

        public JenkinsJob() { }

        private string id;
        private string name;
        private string url;
        private string status;
        private bool building;
        private bool temporary;
        private bool removeIfCompleted;
        private Color color;
        private NotifySettings notifySettings;
        private int index;
        private int lastCompletedBuildNumber;
        private string category;

        private const string DEFAULT_CATEGORY = "cbam";

        private const string SUCCESS = "SUCCESS";
        private const string ABORTED = "ABORTED";
        private const string FAILURE = "FAILURE";
        private const string UNSTABLE = "UNSTABLE";
        

        [XmlAttribute]
        public string JobId
        {
            get { return id; }
            set { id = value; }
        }

        [XmlAttribute]
        public string JobName
        {
            get { return name; }
            set { name = value; }
        }

        [XmlAttribute]
        public string JobUrl
        {
            get { return url; }
            set { url = value; }
        }

        [XmlIgnore]
        public Color JobStatusColor
        {
            get { return color; }
            set { color = value; }
        }

        [XmlAttribute]
        public bool Building
        {
            get { return building; }
            set { building = value; }
        }

        [XmlAttribute]
        public string JobStatus
        {
            get { return status; }
            set { this.status = value; color = GetColorForStatus(value); }
        }

        [XmlIgnore]
        public string JobInfo
        {
            get { return String.Format("{0}{1}", status, building? " - Build in progress" : String.Empty); }
        }

        [XmlAttribute]
        public bool IsTemporary
        {
            get { return temporary; }
            set { this.temporary = value; }
        }

        [XmlAttribute]
        public bool RemoveIfCompleted
        {
            get { return removeIfCompleted; }
            set { this.removeIfCompleted = value; }
        }

        [XmlAttribute]
        public int Index
        {
            get { return index; }
            set { this.index = value; }
        }

        [XmlAttribute]
        public int LastComletedBuildNumber
        {
            get { return lastCompletedBuildNumber; }
            set { this.lastCompletedBuildNumber = value; }
        }

        [XmlAttribute]
        public string Category
        {
            get { return category; }
            set { this.category = value; }
        }

        public NotifySettings NotifySettings
        {
            get { return notifySettings; }
            set { this.notifySettings = value; }
        }

        private Color GetColorForStatus(string Status)
        {
            switch (Status)
            {
                case SUCCESS: return (Color)App.Current.TryFindResource("color_green");
                case FAILURE: return (Color)App.Current.TryFindResource("color_red");
                case ABORTED: return (Color)App.Current.TryFindResource("color_gray");
                case UNSTABLE: return (Color)App.Current.TryFindResource("color_yellow");
                default:
                    return Colors.White;
            }
        }

        public void UpdateJobStatus(JenkinsWorkflow NewStatus)
        {
            if (temporary)
            {
                WorkflowRun tempJobNewStatus = NewStatus as WorkflowRun;
                this.status = tempJobNewStatus.result;
                this.building = tempJobNewStatus.building;
                this.color = GetColorForStatus(status);
            }
            else
            {
                WorkflowJob jobNewStatus = NewStatus as WorkflowJob;
                if (jobNewStatus.lastCompletedBuild != null)
                {
                    this.status = jobNewStatus.lastCompletedBuild.result;
                    this.lastCompletedBuildNumber = jobNewStatus.lastCompletedBuild.number;
                }
                if (jobNewStatus.lastBuild != null) this.building = jobNewStatus.lastBuild.building;
                this.color = GetColorForStatus(status);
            }
        }

        public bool DoNotify(JenkinsWorkflow NewStatus)
        {
            if (temporary)
            {
                WorkflowRun tempJobNewStatus = NewStatus as WorkflowRun;
                if ((notifySettings.NotifyWhenJobStateChanges || notifySettings.NotifyWhenBuildIsComplete || notifySettings.NotifyWhenJobBecomesGreen || notifySettings.NotifyWhenJobBecomesRed))
                    if ( this.building != tempJobNewStatus.building) return !tempJobNewStatus.building;
            }
            else
            {
                WorkflowJob jobNewStatus = NewStatus as WorkflowJob;
                if (notifySettings.NotifyWhenJobStateChanges && IsLastBuildCompleted(jobNewStatus)  && this.status != jobNewStatus.lastCompletedBuild.result) return true;
                if (notifySettings.NotifyWhenBuildIsComplete && IsLastBuildCompleted(jobNewStatus)) return true;
                if (notifySettings.NotifyWhenJobBecomesGreen && this.status != jobNewStatus.lastCompletedBuild.result && jobNewStatus.lastCompletedBuild.result == SUCCESS) return true;
                if (notifySettings.NotifyWhenJobBecomesRed && this.status != jobNewStatus.lastCompletedBuild.result && jobNewStatus.lastCompletedBuild.result != SUCCESS) return true;
            }

            return false;
        }

        private bool IsLastBuildCompleted(WorkflowJob NewStatus)
        {
            return this.lastCompletedBuildNumber != NewStatus.lastCompletedBuild.number;
        }


        public void RemoveJob()
        { 
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            JenkinsJob job = obj as JenkinsJob;
            if (job == null) return false;
            return this.id == job.id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
