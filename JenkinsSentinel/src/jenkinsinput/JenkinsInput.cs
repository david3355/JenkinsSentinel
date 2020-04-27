using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src.jenkinsinput
{
    public class JenkinsInput
    {
        public JenkinsInput()
        {
            parameter = new List<JenkinsParameters>();
        }

        public List<JenkinsParameters> parameter;

        public void Add(string Name, string Value)
        {
            parameter.Add(new JenkinsParameters() { name = Name, value = Value });
        }
    }

    public class JenkinsParameters
    {
        public string name;
        public string value;
    }
}
