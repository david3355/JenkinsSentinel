using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsdata
{
    public class WorkflowJob : JenkinsWorkflow
    {
        public String name;
        public String url;
        public List<WorkflowRun> builds;
        public WorkflowRun lastCompletedBuild;
        public WorkflowRun lastBuild;
    }
}
