using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JenkinsSentinel.src
{
    [XmlRootAttribute("NotifySettings", IsNullable = false)]
    public class NotifySettings
    {
        [XmlAttribute]
        public bool NotifyWhenJobBecomesGreen { get; set; }

        [XmlAttribute]
        public bool NotifyWhenJobBecomesRed { get; set; }

        [XmlAttribute]
        public bool NotifyWhenJobStateChanges { get; set; }

        [XmlAttribute]
        public bool NotifyWhenBuildIsComplete { get; set; }
    }
}
