using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsSentinel.src
{
    public interface ISentinelEvents
    {
        void ConnectionError(string ErrorMessage, string ErrorDetails);
        void ConnectionWorks();
        void JobRemoved(JenkinsJob Job);
        void CheckCycleExpired();
        void CheckCycleFinished(bool Success);
    }
}
