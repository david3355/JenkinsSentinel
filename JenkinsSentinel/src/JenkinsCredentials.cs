using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JenkinsSentinel.src
{
    [XmlRootAttribute("JenkinsCredentials", IsNullable = false)]
    public class JenkinsCredentials
    {
        public JenkinsCredentials(string Username, string Password)
        {
            this.username = Username;
            this.password = Password;
        }

        public JenkinsCredentials()
        {}

        private string username, password;

        [XmlAttribute]
        public string Username { get { return username; } set { this.username = value; } }

        [XmlAttribute]
        public string Password { get { return password; } set { this.password = value; } }
    }
}
