using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace SluggaManager.Services
{
    public abstract class APIServiceProxy
    {
        protected string ServiceCall(string url)
        {
            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.Method = "GET";
            string result = "";
            try
            {
                var json = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
                result = json;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error in Service Call: {ex}");
                result = ex.Message;
            }
            return result;
        }
    }
}