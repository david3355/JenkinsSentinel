using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JenkinsSentinel.src.jenkinsdata;
using System.Net;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Timers;

namespace JenkinsSentinel.src
{
    [XmlRootAttribute("Sentinel", IsNullable = false)]
    public class Sentinel
    {
        public Sentinel(int InspectFrequencyMS, JenkinsCredentials Credentials, ISentinelEvents EventHandler)
        {
            this.inspectFrequency = InspectFrequencyMS;
            this.eventHandler = EventHandler;
            this.client = new JenkinsClient(Credentials, EventHandler);
            this.jobs = new List<JenkinsJob>();
            this.checkTimer = new Timer(InspectFrequencyMS);
            this.checkTimer.Elapsed += CheckJobs;
            this.lastIndex = 0;
        }

        public Sentinel(int InspectFrequencyMS, ISentinelEvents EventHandler)
            : this(InspectFrequencyMS, null, EventHandler)
        {
        }

        public Sentinel()
        { }


        private bool watching;
        private List<JenkinsJob> jobs;
        private int inspectFrequency;
        private event ReportCallback eventListeners;
        private JenkinsClient client;
        private Timer checkTimer;
        private ISentinelEvents eventHandler;
        private int lastIndex;


        [XmlArrayAttribute("Jobs")]
        public List<JenkinsJob> Jobs
        {
            get { return jobs; }
            set { jobs = value; }
        }

        [XmlAttribute]
        public int LastIndex
        {
            get { return lastIndex; }
            set { this.lastIndex = value; }
        }

        [XmlAttribute]
        public int InspectFrequency
        {
            get { return inspectFrequency; }
            set { this.inspectFrequency = value; }
        }

        void CheckJobs(object sender, ElapsedEventArgs e)
        {
            eventHandler.CheckCycleExpired();
            bool success = true;
            foreach (JenkinsJob job in jobs)
            {
                success &= UpdateJob(job);
            }
            eventHandler.CheckCycleFinished(success);
        }

        public bool UpdateJob(JenkinsJob Job, bool Notify=true)
        {
            if (Job.IsTemporary && !Job.Building)
            {
                if (Job.RemoveIfCompleted) RemoveJob(Job);
                return true;   // Temporary jobs done are not updated
            }

            JenkinsWorkflow updatedJob = GetJobReport(Job);
            if (updatedJob == null) return false;
            JenkinsJob oldJob = Job.Clone() as JenkinsJob;
            Job.UpdateJobStatus(updatedJob);
            if (Notify && oldJob.DoNotify(updatedJob)) NotifyListeners(Job);
            return true;
        }

        public void RemoveJob(JenkinsJob Job)
        {
            jobs.Remove(Job);
            eventHandler.JobRemoved(Job);
        }

        public bool IsWatching()
        {
            return watching;
        }

        public void StartWatching()
        {
            this.checkTimer.Start();
            watching = true;
        }

        public void StopWatching()
        {
            this.checkTimer.Stop();
            watching = false;
        }

        public void SetCredentials(JenkinsCredentials Credentials)
        {
            this.client = new JenkinsClient(Credentials, eventHandler);
        }

        public void AddNewJob(JenkinsJob NewJob)
        {
            JenkinsWorkflow job = GetJobReport(NewJob);
            if (job == null) return;
            NewJob.UpdateJobStatus(job);
            this.jobs.Add(NewJob);
            lastIndex = NewJob.Index;
        }

        public bool MoveJobIndexUp(JenkinsJob Job)
        {
            int index = Job.Index;
            if (index == 0) return false;
            int biggest_lower_index = 0;
            JenkinsJob bliJob = null;
            foreach (JenkinsJob j in jobs)
            {
                if (j.Index >= biggest_lower_index && j.Index < index)
                {
                    biggest_lower_index = j.Index;
                    bliJob = j;
                }
            }
            if (bliJob != null)
            {
                bliJob.Index = Job.Index;
                Job.Index = biggest_lower_index;
                return true;
            }
            return false;
        }

        public bool MoveJobIndexDown(JenkinsJob Job)
        {

            int index = Job.Index;
            if (index == lastIndex) return false;
            int lowest_bigger_index = lastIndex;
            JenkinsJob lbiJob = null;
            foreach (JenkinsJob j in jobs)
            {
                if (j.Index <= lowest_bigger_index && j.Index > index)
                {
                    lowest_bigger_index = j.Index;
                    lbiJob = j;
                }
            }
            if (lbiJob != null)
            {
                lbiJob.Index = Job.Index;
                Job.Index = lowest_bigger_index;
                return true;
            }
            return false;
        }

        public void AddReportListener(ReportCallback Listener)
        {
            eventListeners += Listener;
        }

        public void RemoveReportListener(ReportCallback Listener)
        {
            eventListeners -= Listener;
        }

        private void NotifyListeners(JenkinsJob UpdatedJob)
        {
            if (eventListeners != null) eventListeners(UpdatedJob);
        }

        private string GetJobApiUrl(string JobUrl)
        {
            return String.Format("{0}/api", JobUrl);
        }

        private string GetJobReportUrl(string JobName, bool Temporary)
        {
            //string query = "name,url,color,healthReport[iconUrl,description],lastCompletedBuild[result,timestamp,culprits[fullName],actions[claimed,claimedBy,reason,failCount,totalCount]],lastBuild[building,timestamp,testReport],builds[number,building,timestamp,result,duration]";
            string query = "name,url,color,builds[number,building,timestamp,result,duration],lastCompletedBuild[result,timestamp,number],lastBuild[building,timestamp,result,number]";
            if (Temporary) query = "displayName,url,result,building,number,duration";
            return String.Format("{0}/json?tree={1}", GetJobApiUrl(JobName), query);
        }

        public JenkinsWorkflow GetJobReport(JenkinsJob Job)
        {
            string jobQueryUrl = GetJobReportUrl(Job.JobUrl, Job.IsTemporary);
            try
            {
                HttpWebResponse httpWebResponse = client.Get(jobQueryUrl);
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        string responseData = reader.ReadToEnd();
                        JenkinsWorkflow resultData = null;
                        if (Job.IsTemporary) resultData = (WorkflowRun)JsonConvert.DeserializeObject(responseData, typeof(WorkflowRun));
                        else resultData = (WorkflowJob)JsonConvert.DeserializeObject(responseData, typeof(WorkflowJob));
                        eventHandler.ConnectionWorks();
                        return resultData;
                    }
                }
            }
            catch (Exception e)
            {
                eventHandler.ConnectionError("Jenkins data error!", e.Message);
            }
            return null;

        }
    }
}
