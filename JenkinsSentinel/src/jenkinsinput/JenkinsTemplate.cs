using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsinput
{
    public enum ParamType { MAIN, ADVANCED, NON_EDITABLE }

    public abstract class JenkinsTemplate
    {
        public JenkinsTemplate(string BranchName, string DefaultCloud)
        {
            this.branch = BranchName;
            this.defaultCloud = DefaultCloud;
        }

        protected JenkinsInput input;
        private string branch;
        protected string defaultCloud;

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
        public Jumpstart(string JenkinsUsername, string DefaultCloud, string BranchName="master") :base(BranchName, DefaultCloud)
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
                new ListParameter("TARGET_CLOUD", ParamType.MAIN, new List<string> {defaultCloud, "nickel"}),
                new BooleanParameter("DEBUGGABLE", ParamType.MAIN, true),
                new BooleanParameter("VERBOSE_LOGS", ParamType.MAIN, true),
                new BooleanParameter("UPLOAD_LCN_RECEIVER", ParamType.ADVANCED, true),
                new BooleanParameter("ENABLE_FM", ParamType.ADVANCED, false),
                new TextParameter("FLAVOR", ParamType.ADVANCED, "m1.large")
                
            };
        }

        /*
        IMAGE_NAME	
        MULTINODE
        ONLY_IPV4
        FULL_IPV6
        APPLY_PATCH
        CRM_LATEST
        */
    }

    public class ITEnvironment : JenkinsTemplate
    {
        public ITEnvironment(string JenkinsUsername, string DefaultCloud, string BranchName = "master")
            : base(BranchName, DefaultCloud)
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
            get { return String.Format("https://build13.cci.nokia.net/job/CloudBand/job/CBAM/job/{0}/job/90-cmd-exec", Branch); }
        }
    }

    public class BuildBot : JenkinsTemplate
    {
        public BuildBot(string JenkinsUsername, string DefaultCloud, string BranchName = "master")
            : base(BranchName, DefaultCloud)
        {
            this.user = JenkinsUsername;
        }

        private string user;



        public override List<JobEditorParameter> GetMainParameters()
        {
            return new List<JobEditorParameter>()
            {
                // TODO add environment variables to COMMAND
                //new TextParameter("COMMAND", ParamType.NON_EDITABLE, "make -j 6 -l 48 cbam-jumpstart.jumpstart"),
            };
        }

        public override string TemplateName { get { return "BuildBot"; } }

        public override string JobUrl
        {
            get { return String.Format("https://build13.cci.nokia.net/job/CloudBand/job/CBAM/job/{0}/job/90-pipe-bb-{0}", Branch); }
        }
    }
}
