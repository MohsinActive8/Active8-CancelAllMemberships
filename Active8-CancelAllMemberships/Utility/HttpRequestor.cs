using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Active8_CancelAllMemberships.Utility
{
    public class HttpRequestor
    {
        public string ResponseFull = "";
        public string ResponseStatusDescription = "";
        public int CustomTimeoutInMs = 0;
        public WebHeaderCollection ResponseHeaders = null;

        public string ResponseStatus => this.ResponseStatusCode.ToString();

        public HttpStatusCode ResponseStatusCode = HttpStatusCode.Unused;
        public string ContentType = "application/x-www-form-urlencoded";
        public string UserAgent = "Fusion Framework";
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        /// <summary>
        /// This is a shortened request with some default parameters. It is setup for use with WCF Services.
        /// By default, it sets the content type to application/json and expects postData to be JSON as well. It also only allows for POST, not GET.
        /// </summary>
        /// <param name="url">url to post to</param>
        /// <param name="postData">JSON representation of the data to post.</param>
        /// <param name="username">username, if needed</param>
        /// <param name="password">password, if needed</param>
        /// <returns>error messaging</returns>
        public string Request(string url, string postData = "", string username = "", string password = "")
        {
            this.ContentType = "application/json";
            return this.Request(url, "POST", username, password, postData);
        }

        /// <summary>
        ///  The parameters below provide a sample call for TEST.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="postdata"></param>
        /// <returns>POST: Expection or empty string. GET: Response or Expection.</returns>
        public string Request(string url, string method, string username, string password, string postdata, Dictionary<string, string> customHeaders = null, string contentType = "")
        {
            string error = "";
            WebResponse response = null;
            Stream datastream = null;

            try
            {
                //create a web request using the appropriate method
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ServicePoint.Expect100Continue = false;

                if (this.CustomTimeoutInMs > 0)
                {
                    request.Timeout = this.CustomTimeoutInMs;
                }

                if (!string.IsNullOrEmpty(username))
                {
                    request.Credentials = new BasicCredentials(username, password);
                }

                //set request headers
                if (customHeaders != null)
                {
                    foreach (KeyValuePair<string, string> header in customHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(contentType))
                {
                    this.ContentType = contentType;
                }

                //((HttpWebRequest)request).UserAgent = this.UserAgent;
                request.Method = method;

                if (method == "POST" || method == "PUT")
                {
                    //encode the posted data as a byte array
                    //UTF8Encoding encoder = new UTF8Encoding();
                    //byte[] array = encoder.GetBytes(postdata);
                    byte[] array = Encoding.ASCII.GetBytes(postdata);

                    request.ContentType = this.ContentType;
                    request.ContentLength = array.Length;

                    //stream the request and its data
                    datastream = request.GetRequestStream();
                    datastream.Write(array, 0, array.Length);
                }

                //get a response from the server and return the basic status to the ResponseStatus property
                try
                {
                    response = request.GetResponse();
                }
                catch (WebException err)
                {
                    // check for an inner response if a webexception is thrown
                    if (err.Response != null)
                    {
                        response = err.Response;
                    }
                    else
                    {
                        throw new Exception(err.Message);
                    }
                }

                this.ResponseStatusDescription = ((HttpWebResponse)response).StatusDescription;
                this.ResponseStatusCode = ((HttpWebResponse)response).StatusCode;

                if (response.SupportsHeaders)
                {
                    this.ResponseHeaders = response.Headers;
                }

                //stream the response, read the stream, set the ResponseFull property
                datastream = response.GetResponseStream();
                StreamReader reader = new StreamReader(datastream);
                this.ResponseFull = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (datastream != null)
                {
                    datastream.Close();
                    datastream.Dispose();
                }
            }

            if (method == "GET" && error == "")
            {
                return this.ResponseFull;
            }
            else
            {
                return error;
            }
        }
    }

    public class BasicCredentials : ICredentials
    {
        private readonly string UserName = "";
        private readonly string Password = "";

        public BasicCredentials(string username, string password)
        {
            this.UserName = username;
            this.Password = password;
        }

        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            if (!((string.IsNullOrEmpty(this.UserName)) && (string.IsNullOrEmpty(this.Password))))
            {
                NetworkCredential creds = new NetworkCredential(this.UserName, this.Password);
                return creds;
            }
            else
            {
                NetworkCredential creds = new NetworkCredential();
                return creds;
            }
        }
    }
}
