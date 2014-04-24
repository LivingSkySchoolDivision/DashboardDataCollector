using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LSKYDashboardDataCollector.iBoss
{
    public class iBossConnection : IDisposable
    {
        private string Username;
        private string Password;
        private string iBossURL;

        private static string userAgent = "iBoss JSON Connector written by Mark Strendin";
        private static Cookie CachedLoginCookie;
        private static DateTime CachedLoginCookieTime;

        public iBossConnection(string url, string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.iBossURL = url;

            if (this.GetLoginCookie() == null)
            {
                throw new Exception("iBoss login failed - check your username or password");
            }
        }

        /// <summary>
        /// Returns a session ID cookie given the specified username and password. Returns null if the login failed
        /// </summary>
        /// <param name="iBossURL"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Cookie GetLoginCookie()
        {
            try
            {
                if ((CachedLoginCookie == null) || DateTime.Now.Subtract(CachedLoginCookieTime) > new TimeSpan(2, 0, 0))
                {
                    string loginurl = this.iBossURL + @"/ibreports/action/login";
                    string formParameters = string.Format("button=login&username={0}&password={1}", this.Username, this.Password);

                    // Send the login request to the login page
                    HttpWebRequest LoginRequest = (HttpWebRequest)WebRequest.Create(loginurl);
                    LoginRequest.UserAgent = userAgent;
                    LoginRequest.ContentType = "application/x-www-form-urlencoded";
                    LoginRequest.Method = "POST";
                    byte[] LoginBytes = Encoding.ASCII.GetBytes(formParameters);
                    LoginRequest.ContentLength = LoginBytes.Length;
                    using (Stream os = LoginRequest.GetRequestStream())
                    {
                        os.Write(LoginBytes, 0, LoginBytes.Length);
                    }
                    WebResponse LoginResponse = LoginRequest.GetResponse();

                    // Analyze the response - did we get a login page back, or did it redirect us?
                    Stream receiveStream = LoginResponse.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    string webresponseresults = readStream.ReadToEnd();

                    if (webresponseresults.Contains("Please login"))
                    {
                        // A login page was returned
                        return null;
                    }
                    else
                    {
                        // A non-login page was returned
                        // Parse the returned cookie into a cookie object
                        string LoginCookieHeader = LoginResponse.Headers["Set-cookie"]; ;
                        Regex pattern = new Regex(@"JSESSIONID=(.+); Path=/ibreports");
                        Match match = pattern.Match(LoginCookieHeader);
                        string ParsedSessionID = match.Groups[1].Value;
                        if (!string.IsNullOrEmpty(ParsedSessionID))
                        {
                            CachedLoginCookieTime = DateTime.Now;
                            CachedLoginCookie = new Cookie("JSESSIONID", ParsedSessionID);
                            return CachedLoginCookie;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return CachedLoginCookie;
                }
            }
            catch
            {
                return null;
            }
        }

        private string GetRawHTTPContent(string url)
        {
            // Attempt to ignore SSL issues
            ServicePointManager.ServerCertificateValidationCallback = delegate
            { return true; }; // WARNING: accepts all SSL certificates

            HttpWebRequest DataPageRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            DataPageRequest.UserAgent = userAgent;

            if (this.GetLoginCookie() != null)
            {
                Uri cookieTarget = new Uri(this.iBossURL);
                DataPageRequest.CookieContainer = new CookieContainer();
                DataPageRequest.CookieContainer.Add(cookieTarget, this.GetLoginCookie());

                HttpWebResponse DataPageResponse = (HttpWebResponse)DataPageRequest.GetResponse();
                Stream receiveStream = DataPageResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                return readStream.ReadToEnd();
            }
            else
            {
                return string.Empty;
            }

        }

        private static Int64 GetTime()
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }

        public string GetBandwidthConsumersRaw()
        {
            string url = "/ibreports/action/currentActivityBandwidthUsers?username=&timeNow=" + GetTime();
            return GetRawHTTPContent(this.iBossURL + url);
        }

        public List<iBossBandwidthUser> GetBandwidthConsumers()
        {
            List<iBossBandwidthUser> returnMe = new List<iBossBandwidthUser>();

            string[] ConsumersParsed = GetBandwidthConsumersRaw().Split('#');

            for (int x = 0; x < ConsumersParsed.Length; x += 3)
            {
                returnMe.Add(new iBossBandwidthUser(ConsumersParsed[x], ConsumersParsed[x + 1], ConsumersParsed[x + 2]));
            }

            return returnMe;

        }

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed ressources
                }
            }
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}