using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace JenkinsSentinel.src
{
    class Persistor
    {
        private static Persistor instance;

        public static Persistor GetInstance()
        {
            if (instance == null) instance = new Persistor();
            return instance;
        }

        public static string CredentialsFileName = "creds";
        public static string SentinelFileName = "sentinel";

        public void PersistCredentials(JenkinsCredentials Credentials)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(JenkinsCredentials));
            TextWriter writer = new StreamWriter(CredentialsFileName);
            serializer.Serialize(writer, Credentials);
            writer.Close();
        }

        public JenkinsCredentials ReadCredentials()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(JenkinsCredentials));
            FileStream fs = new FileStream(CredentialsFileName, FileMode.Open);
            JenkinsCredentials creds = (JenkinsCredentials)serializer.Deserialize(fs);
            fs.Close();
            return creds;
        }

        public void PersistJobs(Sentinel Sentinel)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Sentinel));
            TextWriter writer = new StreamWriter(SentinelFileName);
            serializer.Serialize(writer, Sentinel);
            writer.Close();
        }

        public Sentinel ReadJobs()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Sentinel));
            FileStream fs = new FileStream(SentinelFileName, FileMode.Open);
            Sentinel sentinel = (Sentinel)serializer.Deserialize(fs);
            fs.Close();
            return sentinel;
        }
    }
}
