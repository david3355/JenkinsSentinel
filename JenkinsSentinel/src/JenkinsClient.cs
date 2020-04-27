using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using JenkinsSentinel.src.jenkinsdata;

namespace JenkinsSentinel.src
{
    public class JenkinsClient
    {
        public JenkinsClient(JenkinsCredentials Credentials, ISentinelEvents EventHandler)
        {
            this.creds = Credentials;
            this.eventHandler = EventHandler;
        }

        private JenkinsCredentials creds;
        private ISentinelEvents eventHandler;

        private HttpWebRequest GetRequest(string Uri)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri);
            httpWebRequest.PreAuthenticate = true;
            //httpWebRequest.Credentials = GetCredential("build13.cci.nokia.net", creds.Username, creds.Password);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", creds.Username, creds.Password)));
            httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            return httpWebRequest;
        }

        public bool Authenticate()
        {
            string testPath = "https://build13.cci.nokia.net/api/json";
            HttpWebResponse response = Get(testPath);
            return response != null && response.StatusCode == HttpStatusCode.OK;
        }

        public HttpWebResponse Post(string Uri, string postData)
        {
            //postData = System.Uri.EscapeDataString(postData);
            postData = String.Format("json={0}", postData);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest httpWebRequest = GetRequest(Uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Count());
            }
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode != HttpStatusCode.Created)
            {
                string message = String.Format("POST failed. Received HTTP status {0}", httpWebResponse.StatusCode);
                throw new Exception(message);
            }
            return httpWebResponse;
        }


        public HttpWebResponse Get(string Uri)
        {
            try
            {
                HttpWebRequest httpWebRequest = GetRequest(Uri);
                httpWebRequest.Method = "GET";
                return (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException e)
            {
                eventHandler.ConnectionError("Jenkins connection error!", e.Message);
            }
            return null;
        }

        /// <summary>
        /// DOES NOT WORK
        /// </summary>
        private CredentialCache GetCredential(string domain, string username, string password)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(String.Format("https://{0}", domain)), "Basic", new NetworkCredential(username, password));
            return credentialCache;
        }
    }
}
