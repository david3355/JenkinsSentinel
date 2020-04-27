using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsinput
{
    public enum ParamType { MAIN, ADVANCED, NON_EDITABLE }

    public abstract class JenkinsTemplate
    {
        public JenkinsTemplate(string BranchName)
        {
            this.branch = BranchName;
        }

        protected JenkinsInput input;
        private string branch;

        public JenkinsInput GetInput()
        {
            return input;
        }

        public void SetInput(JenkinsInput Input)
        {
            input = Input;
        }

        public abstract string TemplateName { get; }
        public abstract string JobUrl { get; }

        public string BuildJobUrl
        {
            get { return String.Format("{0}/build", JobUrl); }
        }

        public abstract List<JobEditorParameter> GetMainParameters();

        public string Branch
        {
            get { return branch; }
            set { this.branch = value; }
        }
    }

    public class Jumpstart : JenkinsTemplate
    {
        public Jumpstart(string JenkinsUsername, string BranchName="master") :base(BranchName)
        {
            this.user = JenkinsUsername;
        }

        private string user;

        public override string TemplateName { get { return "Jumpstart"; } }

        public override string JobUrl
        {
            get { return String.Format("https://build13.cci.nokia.net/job/CloudBand/job/CBAM/job/{0}/job/jumpers/job/80-Jumpstart-Release-{0}", Branch); }
        }


        public override List<JobEditorParameter> GetMainParameters()
        {
            return new List<JobEditorParameter>()
            {
                new TextParameter("INSTANCE_PREFIX", ParamType.MAIN, String.Format("{0}-js-${{BUILD_NUMBER}}", user)),
                new TextParameter("STACK_NAME", ParamType.MAIN, String.Format("{0}-release-{1}-b${{BUILD_NUMBER}}", user, Branch)),
                new BooleanParameter("DEBUGGABLE", ParamType.MAIN, true),
                new ListParameter("TARGET_CLOUD", ParamType.MAIN, new List<string> {"tramboolean", "nickel"}),
                new BooleanParameter("UPLOAD_LCN_RECEIVER", ParamType.ADVANCED, true),
                new BooleanParameter("ENABLE_FM", ParamType.ADVANCED, false),
                new TextParameter("FLAVOR", ParamType.ADVANCED, "m1.large"),
            };
        }

        /*
         	
        STACK_NAME
        IMAGE_NAME	
        FLAVOR	
        TARGET_CLOUD	
        MULTINODE
        DEBUGGABLE
        UPLOAD_LCN_RECEIVER
        ONLY_IPV4
        FULL_IPV6
        VERBOSE_LOGS
        APPLY_PATCH
        ENABLE_FM
        CRM_LATEST
         */
    }

    public class ITEnvironment : JenkinsTemplate
    {
        public ITEnvironment(string JenkinsUsername, string BranchName = "master")
            : base(BranchName)
        {
            this.user = JenkinsUsername;
        }

        private string user;



        public override List<JobEditorParameter> GetMainParameters()
        {
            return new List<JobEditorParameter>()
            {
                // TODO add environment variables to COMMAND
                new TextParameter("COMMAND", ParamType.NON_EDITABLE, "make -j 6 -l 48 cbam-jumpstart.jumpstart"),
            };
        }

        public override string TemplateName { get { return "IT Environment"; } }

        public override string JobUrl
        {
            get { return String.Format("https://build13.cci.nokia.net/job/CloudBand/job/CBAM/job/{0}/job/90-cmd-exec/build", Branch); }
        }
    }
}
