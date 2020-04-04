using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsdata
{
    public class WorkflowRun : JenkinsWorkflow
    {
        public bool building;
        public int duration;
        public int number;
        public string result;
        public long timestamp;
        public string displayName;
        public string url;
    }
}
